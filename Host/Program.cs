using System;
using System.Diagnostics;
using Domain;
using Helper;
using NHibernate;

namespace Host
{
    class Program
    {

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

            HibernatingRhinos.Profiler.Appender.NHibernate.NHibernateProfiler.Initialize();
            setupRedis();
            var conf = Init();
            Stopwatch watch = new Stopwatch();
            using (ISession session = conf.GetSessionFactory().OpenSession())
            {
                using (var trnx = session.BeginTransaction())
                {
                    //Entity e = new Entity();
                    //e.Data = 3;
                    //session.Save(e);
                    //for (int i = 5000; i < 6000; i++)
                    //{
                    //    session.Save(new Entity {Data = i});
                    //}
                    watch.Start();
                    var list = session.QueryOver<Entity>().Cacheable().List();
                    //list.ForEach(x => Console.WriteLine(x.Data));

                    watch.Stop();
                    trnx.Commit();
                }
               
            }
            Console.WriteLine("elapsed: "+watch.Elapsed);
            Console.ReadLine();




            // session.Close();

            //ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
            //ISubscriber sub = redis.GetSubscriber();
            //IDatabase db = redis.GetDatabase(12);
            //db.StringSet("key", "value");
            //string value = db.StringGet("key");
            //Console.WriteLine(value);


        }



    }
}



