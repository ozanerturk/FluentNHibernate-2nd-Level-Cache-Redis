using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using FluentNHibernate.Cfg;
using NHibernate;
using NHibernate.Tool.hbm2ddl;

namespace Helper
{
    public class Configuration
    {
        private  ISessionFactory _sessionFactory;

        public void Initialize()
        {
            foreach (var file in Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.NHibernate.config"))
            {
                BuildSessionFactoryFor(file);
               // BuildSchemaByDroping("Test");
            }
        }

        public ISessionFactory GetSessionFactory()
        {
           
            if ( _sessionFactory == null)
                throw new System.Exception("Unable to locate a SessionFactory  ");

            return _sessionFactory;
        }

        private void BuildSessionFactoryFor(string configurationFilePath)
        {

            var nHibernateMappingAssembly = GetAssemblyName(configurationFilePath);
            ISessionFactory sessionFactory;

            if (nHibernateMappingAssembly != null)
            {
                var assembly = Assembly.Load(nHibernateMappingAssembly);
                var cfg = new global::NHibernate.Cfg.Configuration();
                cfg.Configure(configurationFilePath);
                sessionFactory =
                    Fluently.Configure(cfg).Mappings(m =>
                    {
                        m.FluentMappings.AddFromAssembly(assembly).Conventions.AddAssembly(assembly);

                    }).BuildSessionFactory();
            }
            else
            {
                var cfg = new global::NHibernate.Cfg.Configuration();
                cfg.Configure(configurationFilePath);
                sessionFactory = Fluently.Configure(cfg).BuildSessionFactory();

                Fluently.Configure(cfg);
            }
            _sessionFactory= sessionFactory;

             Console.WriteLine("NHibernate Initiated");


        }

        /// <summary>
        /// Only development phase usage do not use production code
        /// </summary>
        /// <param name="factoryKey"></param>
        private void BuildSchemaByDroping(string factoryKey)
        {
            var filePath = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, factoryKey + ".NHibernate.config")?.FirstOrDefault();
            if (string.IsNullOrEmpty(filePath))
            {
                Console.WriteLine(factoryKey + ".NHibernate.config file not found for BuildSchemaDroping!");
                return;
            }
            var nHibernateMappingAssembly = GetAssemblyName(filePath);

            var cfg = new global::NHibernate.Cfg.Configuration();
            cfg.Configure(filePath);
            FluentConfiguration a = Fluently.Configure(cfg).Mappings(m =>
            {
                var assembly = Assembly.Load(nHibernateMappingAssembly);
                m.HbmMappings.AddFromAssembly(assembly);
                m.FluentMappings.AddFromAssembly(assembly).Conventions.AddAssembly(assembly);
            });

            SchemaExport schema = new SchemaExport(a.BuildConfiguration());

            if (schema != null)
                schema.Execute(true, true, false);
        }

        private static string GetAssemblyName(string configurationFilePath)
        {
            var nConfig = new XmlDocument();
            nConfig.Load(configurationFilePath);
            XmlNode root = nConfig.DocumentElement;
            if (root == null)
                return null;

            var selectSingleNode = root.SelectSingleNode("//configuration//CustomConfiguration//property//text()");
            if (selectSingleNode == null)
                return null;

            return selectSingleNode.Value;
        }
    }
}
