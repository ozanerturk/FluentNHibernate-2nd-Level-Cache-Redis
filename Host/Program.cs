using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Domain;
using Helper;
using NHibernate;
using NHibernate.SqlCommand;

namespace Host
{
    class Program
    {
        private static Configuration config;

        static Configuration Init()
        {
            Configuration confg = new Configuration();
            confg.Initialize();
            return confg;

        }

        static void setupRedis()
        {
            var cacheFactory = new NHibernateCacheFactory();
            cacheFactory.SetupRedisCacheProvider(
                clientName: "test",
                serviceName: "test",
                databaseId: 13);
        }

        static void Main(string[] args)
        {
           //  HibernatingRhinos.Profiler.Appender.NHibernate.NHibernateProfiler.Initialize();
            setupRedis();
            config = Init();

            IList<Entity> result = new List<Entity>();

            //Save<Entity>(save =>
            //{
            //    for (int i = 0; i < 5000; i++)
            //    {
            //        var entity = new Entity { Data = i };
                   
            //        if (i % 2 == 0)
            //        {
                       
            //            entity.SubEntity = new Entity() {Data = i*10};
            //        }
            //        else if (i % 3 == 0)
            //        {
            //            entity.SubEntity = new Entity() { Data = i * 10 };
            //            entity.SubEntity.SubEntity = new Entity() { Data = i * 10 };
            //        }
            //        save.Add(entity);
            //    }

            //});

           QueryOver<Entity>(queryOver =>
           {
               Entity entityAlias=null;
                result = queryOver.Fetch(x=>x.SubEntity).Eager.Cacheable().List();

           });

            foreach (var item in result)
            {
                //Console.WriteLine(item.Data);
            }

            Console.ReadLine();

        }

        static void SessionScope(Action<ISession> action)
        {
            Stopwatch watch = new Stopwatch();

            using (ISession session = config.GetSessionFactory().OpenSession())
            {
                using (var trnx = session.BeginTransaction())
                {

                    watch.Start();
                    action.Invoke(session);
                    watch.Stop();
                    Console.WriteLine("elapsed: " + watch.Elapsed);
                    trnx.Commit();
                    Console.WriteLine("Commited");
                }

            }
        }
        static void StatelessSessionScope(Action<IStatelessSession> action)
        {
            Stopwatch watch = new Stopwatch();

            using (IStatelessSession session = config.GetSessionFactory().OpenStatelessSession())
            {
                using (var trnx = session.BeginTransaction())
                {

                    watch.Start();
                    action.Invoke(session);
                    watch.Stop();
                    Console.WriteLine("elapsed: " + watch.Elapsed);
                    trnx.Commit();
                }

            }
        }

        static void QueryOver<T>(Action<IQueryOver<T,T>> action) where T : class
        {
            SessionScope(session =>
            {
                action.Invoke(session.QueryOver<T>());
            });
        }

        static void Save<T>(Action<IList<T>> action) where T : class
        {
            List<T> entities = new List<T>();

            SessionScope(session =>
            {
                action.Invoke(entities);
                foreach (var item in entities)
                {
                    session.SaveOrUpdate(item);
                }

            });
        }

    }
}



