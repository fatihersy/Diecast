
using System.Runtime.Serialization;

namespace DieEditor.Components
{
    interface IMSComponent { }

    [DataContract]
    abstract class GameComponent : ViewModelBase
    {
        [DataMember]
        public GameEntity Owner { get; private set; }

        public GameComponent(GameEntity owner)
        {
            System.Diagnostics.Debug.Assert(owner != null);
            Owner = owner;
		}
	}

    abstract class MSComponent<T> : ViewModelBase, IMSComponent where T : GameComponent { 

    }
}