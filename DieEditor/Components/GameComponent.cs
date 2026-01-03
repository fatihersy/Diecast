
using System.Runtime.Serialization;

namespace DieEditor.Components
{
    [DataContract]
    public class GameComponent : ViewModelBase
    {
        [DataMember]
        public GameEntity Owner { get; private set; }

        public GameComponent(GameEntity owner)
        {
            System.Diagnostics.Debug.Assert(owner != null);
            Owner = owner;
		}
	}
}