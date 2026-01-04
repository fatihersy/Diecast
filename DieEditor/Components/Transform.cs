
using System.Numerics;
using System.Runtime.Serialization;

namespace DieEditor.Components
{
    [DataContract]
	class Transform : GameComponent
    {
        private Vector3 _position = Vector3.Zero;
        [DataMember]
        public Vector3 Position
        {
            get => _position;
            set
            {
                if (_position != value)
                {
                    _position = value;
                    OnPropertyChanged(nameof(Position));
                }
            }
		}

		private Vector3 _rotation = Vector3.Zero;
		[DataMember]
        public Vector3 Rotation
        {
            get => _rotation;
            set
            {
                if (_rotation != value)
                {
                    _rotation = value;
                    OnPropertyChanged(nameof(Rotation));
                }
            }
		}

		private Vector3 _scale = Vector3.One;
		[DataMember]
        public Vector3 Scale
        {
            get => _scale;
            set
            {
                if (_scale != value)
                {
                    _scale = value;
                    OnPropertyChanged(nameof(Scale));
                }
            }
		}

		public Transform(GameEntity owner) : base(owner)
		{

        }
    }
}
