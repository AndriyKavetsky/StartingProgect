using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartingProgect
{
    public class Domain
    {
        private BoundaryConnection boundaries;
        private Mesh mesh;
        //клас фізичні властивості області

        public Domain(Mesh mesh, BoundaryConnection boundaries)
        {
            this.mesh = mesh;
            this.boundaries = boundaries;
        }

        public object Clone()
        {
            return new Domain((Mesh)this.mesh.Clone(), (BoundaryConnection)this.boundaries.Clone());
        }

        public BoundaryConnection Boundaries
        {
            get
            {
                return boundaries;
            }
            set
            {
                boundaries = value;
            }
        }
        public Mesh Meshes
        {
            get
            {
                return mesh;
            }
            set
            {
                mesh = value;
            }
        }

    }
}
