using NetAdvcancedLiveCoding._2021._09.Model;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;

namespace NetAdvcancedLiveCoding._2021._09
{
    class SearchRequest
    {
        public string Entity { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }

        public SearchRequest And { get; set; }
        public SearchRequest Or { get; set; }
    }


    class Visitor : ExpressionVisitor
    {
        public SearchRequest Request { get; private set; } = new SearchRequest();

        protected override Expression VisitConstant(ConstantExpression node)
        {
            if (Request.Entity == null)
            {
                Request.Entity = node.Value.GetType().GenericTypeArguments[0].Name;
            }
            else
                Request.Value = node.Value.ToString();
            return base.VisitConstant(node); 
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            if (node.NodeType == ExpressionType.AndAlso || node.NodeType == ExpressionType.OrElse)
            {
                var visitor = new Visitor();
                visitor.Request = Request;
                visitor.Visit(node.Left);

                var visitor2 = new Visitor();
                visitor2.Request.Entity = Request.Entity;
                visitor2.Visit(node.Right);

                Request.And = node.NodeType == ExpressionType.AndAlso ? visitor2.Request : null;
                Request.Or = node.NodeType == ExpressionType.Or ? visitor2.Request : null;

                return node;
            }
            return base.VisitBinary(node);
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            Request.Key = node.Member.Name;
            return base.VisitMember(node);
        }

        public override Expression Visit(Expression node)
        {
            return base.Visit(node);
        }
    }
    class QueryProvider : IQueryProvider
    {
        public IQueryable CreateQuery(Expression expression)
        {
            throw new NotImplementedException();
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            var v = new Visitor();
            v.Visit(expression);

           // HttpClint.Post(JsonConvert.SerializeObject(v.Request));
            return null;
        }

        public object Execute(Expression expression)
        {
            throw new NotImplementedException();
        }

        public TResult Execute<TResult>(Expression expression)
        {
            throw new NotImplementedException();
        }
    }

    class MyQueryable<T> : IQueryable<T>
    {
        public MyQueryable(IQueryProvider provider)
        {
            Provider = provider;
            Expression = Expression.Constant(this);
        }

        public Type ElementType => typeof(T);

        public Expression Expression { get; }

        public IQueryProvider Provider { get; }

        public IEnumerator<T> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }

    class Program
    {
        public class ExpressionBuilder<T>
        {
            private class Mapper
            {
                public Func<SearchRequest, bool> When;
                public Expression<Func<T, SearchRequest, bool>> How;
            }

            private List<Mapper> mappers = new List<Mapper>();

            public ExpressionBuilder()
            {

            }

            

            public Expression<Func<T, bool>> Build(SearchRequest payload)
            {
                var param = Expression.Parameter(typeof(T));

                var mapper = mappers.FirstOrDefault(x => x.When(payload));
                Expression body;
                if (mapper != null)
                {
                    body = Expression.Invoke(mapper.How, param, Expression.Constant(payload));
                }
                else 
                    body = Expression.Equal(Expression.PropertyOrField(param, payload.Key), Expression.Constant(payload.Value));
                if (payload.And != null)
                {

                    // &=
                    // &
                    // &&
                    // x > 9 && Sin
                    body = Expression.AndAlso(body, Expression.Invoke(Build(payload.And), param));
                }
                if (payload.Or != null)
                {
                    body = Expression.OrElse(body, Expression.Invoke(Build(payload.Or), param));

                }
                return Expression.Lambda<Func<T, bool>>(body, param);
            }

            public void When(Func<SearchRequest, bool> when, Expression<Func<T, SearchRequest, bool>> how)
            {
                mappers.Add(new Mapper() { When = when, How = how });
            }
        }

        static void Main(string[] args)
        {
            var q = new MyQueryable<Customer>(new QueryProvider());
            q.Where(x => x.LastName == "John" && x.FirstName == "Doe").ToList();
            
            var payload = JsonConvert.DeserializeObject<SearchRequest>(File.ReadAllText("payload.json"));
            Model.AdvancedLiveCodingContext dbCtx = new Model.AdvancedLiveCodingContext();
            var builder = new ExpressionBuilder<Model.Customer>();
            builder.When(x => x.Key == "CustomerPhone", (p, x) => p.CustomerPhones.Any(ph => ph.PhoneNumber.StartsWith(x.Value)));
            var filter = builder.Build(payload);
            var customers = dbCtx.Customers
                //.Where(x => x.LastName == "Frazer")
                //.Where(x => x.LastName == "Frazer" && x.FirstName == "Nathaniel" || x.CustomerPhones.Any(y => y.PhoneNumber.StartsWith("(195) 947-5497")))
                .Where(filter).ToList();
                                
        }


    }
}
