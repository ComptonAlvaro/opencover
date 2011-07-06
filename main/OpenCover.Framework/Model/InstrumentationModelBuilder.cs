﻿//
// OpenCover - S Wilde
//
// This source code is released under the MIT License; see the accompanying license file.
//
using System;
using System.Diagnostics;
using System.Linq;
using OpenCover.Framework.Symbols;

namespace OpenCover.Framework.Model
{
    internal class InstrumentationModelBuilder : IInstrumentationModelBuilder
    {
        private readonly ISymbolManager _symbolManager;

        /// <summary>
        /// Standard constructor
        /// </summary>
        /// <param name="symbolManager">the symbol manager that will provide the data</param>
        public InstrumentationModelBuilder(ISymbolManager symbolManager)
        {
            _symbolManager = symbolManager;
        }

        public Module BuildModuleModel()
        {
            var module = new Module {ModuleName = _symbolManager.ModuleName, FullName = _symbolManager.ModulePath, Files = _symbolManager.GetFiles()};
            module.Classes = _symbolManager.GetInstrumentableTypes();
            foreach (var @class in module.Classes)
            {
                BuildClassModel(@class, module.Files);
            }

            return module;
        }

        public bool CanInstrument
        {
            get { return _symbolManager.SourceAssembly != null; }
        }

        private void BuildClassModel(Class @class, File[] files)
        {
            var methods = _symbolManager
                .GetConstructorsForType(@class, files)
                .Union(_symbolManager.GetMethodsForType(@class, files));

            foreach (var method in methods)
            {
                method.SequencePoints = _symbolManager.GetSequencePointsForToken(method.MetadataToken);
            }

            @class.Methods = methods.Where(method => method.SequencePoints != null).ToArray();
        }
    }
}
