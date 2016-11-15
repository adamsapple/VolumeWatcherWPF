﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.IO;
using System.Reflection;

namespace Moral
{
    public class AssemblyInfo
    {
        private readonly Assembly assembly;

        public AssemblyInfo() : this(Assembly.GetEntryAssembly()){ }

        public AssemblyInfo(Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException("assembly");
            this.assembly = assembly;
        }

        /// <summary>
        /// Gets the title property
        /// </summary>
        public string ProductTitle
        {
            get
            {
                return GetAttributeValue<AssemblyTitleAttribute>(a => a.Title,
                        Path.GetFileNameWithoutExtension(assembly.CodeBase));
            }
        }

        /// <summary>
        /// Gets the application's version
        /// </summary>
        public string Version
        {
            get
            {
                string result = string.Empty;
                Version version = assembly.GetName().Version;
                if (version != null)
                    return version.ToString();
                else
                    return "1.0.0.0";
            }
        }

        public string ProductVersion
        {
            get
            {
                var assm = Assembly.GetExecutingAssembly();
                // AssemblyInformationalVersion属性を取得する
                var assemblyInformationalVersion = (AssemblyInformationalVersionAttribute)Attribute.GetCustomAttributes(assm, typeof(AssemblyInformationalVersionAttribute)).First();

                return assemblyInformationalVersion.InformationalVersion;
            }
        }

        public string BuildDateTime
        {
            get
            {
                string result = string.Empty;
                var version = assembly.GetName().Version;

                // バージョンが取得できない場合は
                if (version == null)
                {
                    return string.Empty;
                }
                var build      = version.Build;
                var revision   = version.Revision;
                var baseDate   = new DateTime(2000, 1, 1);
                return baseDate.AddDays(build).AddSeconds(revision * 2).ToString();
            }
        }

        /// <summary>
        /// Gets the description about the application.
        /// </summary>
        public string Description
        {
            get { return GetAttributeValue<AssemblyDescriptionAttribute>(a => a.Description); }
        }


        /// <summary>
        ///  Gets the product's full name.
        /// </summary>
        public string Product
        {
            get { return GetAttributeValue<AssemblyProductAttribute>(a => a.Product); }
        }

        /// <summary>
        /// Gets the copyright information for the product.
        /// </summary>
        public string Copyright
        {
            get { return GetAttributeValue<AssemblyCopyrightAttribute>(a => a.Copyright); }
        }

        /// <summary>
        /// Gets the company information for the product.
        /// </summary>
        public string Company
        {
            get { return GetAttributeValue<AssemblyCompanyAttribute>(a => a.Company); }
        }

        protected string GetAttributeValue<TAttr>(Func<TAttr,
            string> resolveFunc, string defaultResult = null) where TAttr : Attribute
        {
            object[] attributes = assembly.GetCustomAttributes(typeof(TAttr), false);
            if (attributes.Length > 0)
                return resolveFunc((TAttr)attributes[0]);
            else
                return defaultResult;
        }
    }
}
