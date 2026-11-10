using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Domain.Common
{
    public abstract class Entity
    {
        
        protected Entity()
        {
            
        }


        public Guid Id { get; private set; } = Guid.CreateVersion7();

      
        
    


}
}
