namespace TinyMVC.Modules.Networks.Buffers {
    internal sealed class NetActionBuffer {
        public readonly ushort type;
        public readonly ushort locationId;
        public float directionX;
        public float directionZ;
        
        public NetActionBuffer(ushort type, ushort location, float x, float z) {
            this.type = type;
            locationId = location;
            directionX = x;
            directionZ = z;
        }
        
        public void UpdateDirection(float x, float z) {
            directionX = x;
            directionZ = z;
        }
        
        public bool IsCurrent(ushort typeValue, ushort locationValue) => typeValue == type && locationValue == locationId;
    }
}