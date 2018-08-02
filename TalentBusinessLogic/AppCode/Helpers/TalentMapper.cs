//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using AutoMapper;

//namespace TalentBusinessLogic.IoC
//{
//    public class Source<T>
//    {
//        public T Value { get; set; }
//    }

//    public class Destination<T>
//    {
//        public T Value { get; set; }
//    }

//    public interface ITalentMapper
//    {
//        void Map<TSource, TDestination>(TSource input, ref TDestination returnObject);
//    }

//    public class TalentAutoMapper : ITalentMapper
//    {
//        protected void Map<TSource, TDestination>(TSource input, ref TDestination returnObject)
//        {
//            Type t = typeof(Source<TSource>);
//            Type f = typeof(Destination<TDestination>);

//            Mapper.Initialize(cfg =>
//            {
//                cfg.CreateMap<Source<TSource>, Destination<TDestination>>();
//            });


//            var source = new Source<TSource>();
//            var dest = Mapper.Map<Source<TSource>, Destination<TDestination>>(source);
//        }
//    }

//    public class TalentMap<TSource, TDestination>
//    {
//        ITalentMapper action = null;
//        public TalentMap(ITalentMapper concreteAction)
//        {
//            this.action = concreteAction;
//        }
//        public void MapObjects(object input, ref object returnObject)
//        {
//            action.Map<TSource, TDestination>(input, ref returnObject);
//        }
//    }
//}
