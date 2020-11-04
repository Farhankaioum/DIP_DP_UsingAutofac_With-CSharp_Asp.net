﻿using System;
using System.Collections.Generic;
using Autofac;
using Autofac.Core;

namespace AutofacSamples
{
    public interface ILog
    {
        void Write(string message);
    }

    public interface IConsole
    {

    }

    public class ConsoleLog : ILog, IConsole
    {
        public void Write(string message)
        {
            Console.WriteLine(message);
        }
    }

    public class EmailLog : ILog
    {
        private const string adminEmail = "admin@foo.com";

        public void Write(string message)
        {
            Console.WriteLine($"Email sent to {adminEmail} : {message}");
        }
    }

    public class Engine
    {
        private ILog log;
        private int id;

        public Engine(ILog log)
        {
            this.log = log;
            id = new Random().Next();
        }

        public Engine(ILog log, int id)
        {
            this.log = log;
            this.id = id;
        }

        public void Ahead(int power)
        {
            log.Write($"Engine [{id}] ahead {power}");
        }
    }

    public class SMSLog : ILog
    {
        private string phoneNumber;

        public SMSLog(string phoneNumber)
        {
            this.phoneNumber = phoneNumber;
        }

        public void Write(string message)
        {
            Console.WriteLine($"SMS to {phoneNumber} : {message}");
        }
    }

    public class Car
    {
        private Engine engine;
        private ILog log;

        public Car(Engine engine)
        {
            this.engine = engine;
            this.log = new EmailLog();
        }

        public Car(Engine engine, ILog log)
        {
            this.engine = engine;
            this.log = log;
        }

        public void Go()
        {
            engine.Ahead(100);
            log.Write("Car going forward...");
        }
    }

    public class Parent
    {
        public override string ToString()
        {
            return "I am your .....";
        }
    }

    public class Child
    {
        public string Name { get; set; }
        public Parent Parent { get; set; }

        public void SetParent(Parent parent)
        {
            Parent = parent;
        }
    }

    internal class Program
    {
        public static void Main(string[] args)
        {
            var builder = new ContainerBuilder();

            // named parameter
            //      builder.RegisterType<SMSLog>()
            //        .As<ILog>()
            //        .WithParameter("phoneNumber", "+12345678");

            // typed parameter
            //      builder.RegisterType<SMSLog>()
            //        .As<ILog>()
            //        .WithParameter(new TypedParameter(typeof(string), "+12345678"));

            // resolved parameter
            //builder.RegisterType<SMSLog>()
            //     .As<ILog>()
            //     .WithParameter(new ResolvedParameter(
            //         // predicate
            //         (pi, ctx) => pi.ParameterType == typeof(string) && pi.Name == "phoneNumber",
            //          // value
            //         (pi, ctx) => "+12345678"
            //         ));


            //Random random = new Random();
            //builder.Register((c, p) => new SMSLog(p.Named<string>("phoneNumber")))
            //  .As<ILog>();

            builder.RegisterType<Parent>();

            // builder.RegisterType<Child>().PropertiesAutowired(); // if inject property type without constructor
            //builder.RegisterType<Child>()
            //    .WithProperty("Parent", new Parent()); // if inject property type without constructor

            // registration method
            //builder.Register(c =>
            //{
            //    var child = new Child();
            //    child.SetParent(c.Resolve<Parent>());
            //    return child;
            //});
            builder.RegisterType<Child>()
                .OnActivated(e  =>
                {
                    var p = e.Context.Resolve<Parent>();
                    e.Instance.SetParent(p);
                });

            Console.WriteLine("About to build container...");
            var container = builder.Build();

            var parent = container.Resolve<Child>().Parent;
            Console.WriteLine(parent);


            //var log = container.Resolve<ILog>(new NamedParameter("phoneNumber", random.Next().ToString()));
            //log.Write("Testing");
        }
    }
}