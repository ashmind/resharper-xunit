using System.Collections.Generic;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Filtering;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Caches;
using JetBrains.ReSharper.UnitTestExplorer;

namespace XunitContrib.Runner.ReSharper.UnitTestProvider
{
    public class XunitTestElementClass : XunitTestElement
    {
        readonly string assemblyLocation;

        public XunitTestElementClass(IUnitTestProvider provider,
                                     IProjectModelElement project,
                                     string typeName,
                                     string assemblyLocation)
            : base(provider, null, project, typeName)
        {
            this.assemblyLocation = assemblyLocation;
        }

        public string AssemblyLocation
        {
            get { return assemblyLocation; }
        }

        public override IDeclaredElement GetDeclaredElement()
        {
            ISolution solution = GetSolution();

            if (solution == null)
                return null;

            PsiManager manager = PsiManager.GetInstance(solution);
            IList<IPsiModule> modules = PsiModuleManager.GetInstance(solution).GetPsiModules(GetProject());
            IPsiModule projectModule = modules.Count > 0 ? modules[0] : null;
            IDeclarationsScope scope = DeclarationsScopeFactory.ModuleScope(projectModule, false);
            IDeclarationsCache cache = manager.GetDeclarationsCache(scope, true);
            return cache.GetTypeElementByCLRName(GetTypeClrName());
        }

        public override string GetKind()
        {
            return "xUnit.net Test Class";
        }

        public override string GetTitle()
        {
            return new CLRTypeName(GetTypeClrName()).ShortName;
        }

        public override bool Matches(string filter, PrefixMatcher matcher)
        {
            foreach (UnitTestElementCategory category in GetCategories())
                if (matcher.IsMatch(category.Name))
                    return true;

            return matcher.IsMatch(GetTypeClrName());
        }
    }
}