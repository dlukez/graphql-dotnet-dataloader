using System;
using GraphQL.TestApp.Data;
using GraphQL.Types;
using Microsoft.EntityFrameworkCore;

namespace GraphQL.TestApp.Schema
{
    public class StarWarsQuery : ObjectGraphType
    {
        public StarWarsQuery(StarWarsContext db)
        {
            Name = "Query";

            Field<ListGraphType<HumanType>>()
                .Name("humans")
                .Resolve(ctx => db.Humans.ToListAsync());

            Field<ListGraphType<DroidType>>()
                .Name("droids")
                .Resolve(ctx => db.Droids.ToListAsync());
        }
    }
}
