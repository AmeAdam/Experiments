namespace MyXml.PrProj
{
    public class PremiereObjectKey
    {
        public int ObjectID { get; set; }
        public string Name { get; set; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
                return true;
            var other = obj as PremiereObjectKey;
            if (ReferenceEquals(other, null))
                return false;
            return ObjectID == other.ObjectID
                   && Name == other.Name;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (ObjectID * 397) ^ Name.GetHashCode();
            }
        }
    }
}