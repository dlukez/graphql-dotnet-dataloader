using GraphQL.Types;

namespace GraphQL.DataLoader.StarWars.Schema
{
    public class CharacterInterface : InterfaceGraphType
    {
        public CharacterInterface()
        {
            Name = "Character";
            Field<StringGraphType>("name", "The name of the character.");
            Field<ListGraphType<CharacterInterface>>("friends");
            Field<ListGraphType<EpisodeType>>("appearsIn");
        }
    }
}
