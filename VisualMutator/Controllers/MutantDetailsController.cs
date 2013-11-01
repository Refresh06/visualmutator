﻿namespace VisualMutator.Controllers
{
    #region

    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Model;
    using Model.Decompilation;
    using Model.Decompilation.CodeDifference;
    using Model.Mutations.MutantsTree;
    using UsefulTools.Switches;
    using UsefulTools.Wpf;
    using ViewModels;

    #endregion

    public class MutantDetailsController : Controller
    {
        private readonly MutantDetailsViewModel _viewModel;
        private readonly ICodeDifferenceCreator _codeDifferenceCreator;
        private Mutant _currentMutant;
        private MutationTestingSession _session;

        public MutantDetailsController(
            MutantDetailsViewModel viewModel, 
            ICodeDifferenceCreator codeDifferenceCreator)
        {
            _viewModel = viewModel;
            _codeDifferenceCreator = codeDifferenceCreator;


            _viewModel.RegisterPropertyChanged(_=>_.SelectedTabHeader)
                .Where(x=> _currentMutant != null).Subscribe(LoadData);

            _viewModel.RegisterPropertyChanged(_ => _.SelectedLanguage).Subscribe(LoadCode);

        }
        public void LoadDetails(Mutant mutant, MutationTestingSession session)
        {
            _session = session;
            _currentMutant = mutant;

            LoadData(_viewModel.SelectedTabHeader);

        }
        public void LoadData(string header)
        {
            FunctionalExt.Switch(header)
                .Case("Tests", () => LoadTests(_currentMutant))
                .Case("Code", () => LoadCode(_viewModel.SelectedLanguage))
                .ThrowIfNoMatch();   
        }

  

        public async void LoadCode(CodeLanguage selectedLanguage)
        {
            _viewModel.IsCodeLoading = true;
            _viewModel.ClearCode();

            var mutant = _currentMutant;
            var assemblies = _session.OriginalAssemblies;

            if(mutant != null)
            {
                var modules = new ModulesProvider(assemblies.Select(_ => _.AssemblyDefinition).ToList());
                CodeWithDifference diff = await Task.Run(
                    () => _codeDifferenceCreator.CreateDifferenceListing(selectedLanguage,
                    mutant, modules));
                if (diff != null)
                {
                    _viewModel.PresentCode(diff);
                    _viewModel.IsCodeLoading = false;
                }
            }
            
        }

    
        private void LoadTests(Mutant mutant)
        {
            _viewModel.TestNamespaces.Clear();

           

            if (mutant.MutantTestSession.IsComplete)
            {
                _viewModel.TestNamespaces.AddRange(mutant.MutantTestSession.TestNamespaces);
            }
            else
            {
                //_listenerForCurrentMutant = mutant.TestSession.WhenPropertyChanged(_ => _.IsComplete)
                //    .Subscribe(x => _viewModel.TestNamespaces.AddRange(mutant.TestSession.TestNamespaces));
            }
        }


        public MutantDetailsViewModel ViewModel
        {
            get
            {
                return _viewModel;
            }
        }

        public void Clean()
        {
            _currentMutant = null;
           
            _viewModel.IsCodeLoading = false;
            _viewModel.TestNamespaces.Clear();
            _viewModel.SelectedLanguage = CodeLanguage.CSharp;
            _viewModel.ClearCode();

        }
    }
}