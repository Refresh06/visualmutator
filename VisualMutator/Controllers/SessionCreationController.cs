﻿namespace VisualMutator.Controllers
{
    #region

    using System.Linq;
    using Infrastructure;
    using Model;
    using Model.Mutations.Operators;
    using Model.Mutations.Types;
    using Model.Tests;
    using UsefulTools.Core;
    using ViewModels;
    using Views;

    #endregion

    public class SessionCreationController : CreationController<SessionCreationViewModel, ISessionCreationView>
    {
        public SessionCreationController(SessionCreationViewModel viewModel, ITypesManager typesManager,
                                         IOperatorsManager operatorsManager, IHostEnviromentConnection hostEnviroment,
                                         ITestsContainer testsContainer, 
                                         CommonServices svc)
            : base(viewModel, typesManager, operatorsManager, hostEnviroment, testsContainer, svc)
        {
        }

        protected override void AcceptChoices()
        {
            Result = new MutationSessionChoices
                {
                    SelectedOperators = _viewModel.MutationsTree.MutationPackages.SelectMany(pack => pack.Operators)
                        .Where(oper => (bool) oper.IsIncluded).Select(n => n.Operator).ToList(),
                    Assemblies = _viewModel.TypesTreeMutate.Assemblies,
                    ProjectPaths = _typesManager.ProjectPaths.ToList(),
                    SelectedTypes = _typesManager.GetIncludedTypes(_viewModel.TypesTreeMutate.Assemblies),
                    SelectedTests = _testsContainer.GetIncludedTests(_viewModel.TypesTreeToTest.Namespaces),
                    MutantsCreationOptions = _viewModel.MutantsCreation.Options,
                    MutantsTestingOptions = _viewModel.MutantsTesting.Options,
                };
            _viewModel.Close();
        }
    }
}