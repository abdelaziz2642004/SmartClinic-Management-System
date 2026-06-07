namespace Clinic.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string resourse,int id) :base($"{resourse} With Id {id} Not Found ")
        {
            
        }
    }
}
