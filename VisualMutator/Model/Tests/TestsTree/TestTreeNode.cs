﻿namespace VisualMutator.Model.Tests.TestsTree
{
    #region Usings

    using System;
    using System.Linq;
    using System.Windows.Input;

    using CommonUtilityInfrastructure;
    using CommonUtilityInfrastructure.WpfUtils;

    using VisualMutator.Infrastructure;

    #endregion

    public abstract class TestTreeNode : GenericNode
    {
        private ICommand _commandRunTest;

   
        private TestNodeState _state;

        protected TestTreeNode(TestTreeNode parent, string name, bool hasChildren)
            : base(parent, name, hasChildren)
        {
            CommandRunTest = new BasicCommand(Comm);
        }

        public bool HasResults
        {
            get
            {
                return (State == TestNodeState.Failure || State == TestNodeState.Success
                    || State == TestNodeState.Inconclusive);
            }
        }

  
        private string _message;

        public string Message
        {
            set
            {
                if (_message != value)
                {
                    _message = value;
                    RaisePropertyChanged(() => Message);
                }
            }
            get
            {
                return _message;
            }
        }
        public ICommand CommandRunTest
        {
            get
            {
                return _commandRunTest;
            }
            set
            {
                if (_commandRunTest != value)
                {
                    _commandRunTest = value;
                    RaisePropertyChanged(() => CommandRunTest);
                }
            }
        }

        public TestNodeState State
        {
            set
            {
                SetStatus(value, true, true);
            }
            get
            {
                return _state;
            }
        }

        public void SetStatus(TestNodeState value, bool updateChildren, bool updateParent)
        {
            if (_state != value)
            {
                _state = value;

                if (updateChildren && Children != null)
                {

                    if (!(value == TestNodeState.Inactive || value == TestNodeState.Running))
                    {
                        throw new InvalidOperationException("Tried to set invalid state: " + value);
                    }

                    Children.Cast<TestTreeNode>()
                        .Each(c => c.SetStatus(value, updateChildren: true, updateParent: false));
                    
                }
                
                if (updateParent && Parent != null)
                {
                    if (!(value == TestNodeState.Success || value == TestNodeState.Failure 
                        || value == TestNodeState.Inconclusive))
                    {
                        throw new InvalidOperationException("Tried to set invalid state: " + value);
                    }

                    ((TestTreeNode)Parent).UpdateStateBasedOnChildren();
                }
                RaisePropertyChanged(() => State);
            }
        }

     
        private void UpdateStateBasedOnChildren()
        {
            var children = Children.Cast<TestTreeNode>().ToList();

            if(children.All(_ => _.HasResults))
            {
                TestNodeState state;
                if (children.Any(n => n.State == TestNodeState.Failure))
                {
                    state = TestNodeState.Failure;
                }
                else if (children.Any(n => n.State == TestNodeState.Inconclusive))
                {
                    state = TestNodeState.Inconclusive;
                }
                else 
                {
                    state = TestNodeState.Success;
                }
                SetStatus(state, updateChildren: false, updateParent: true);
            }
  

            
           
        }

        public void Comm()
        {
            Name += "!";
        }
    }
}