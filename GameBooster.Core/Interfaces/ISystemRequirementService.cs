using System.ServiceModel;
namespace GameBooster.Core.Interfaces
{
    [ServiceContract] //Bu bir SOAP servisidir der.
    public interface ISystemRequirementService 
    {
        [OperationContract] // Bu methodun dışarıdan çağırılabileceğini anlatır.
        string CheckRequirement(string gameName, int ramAmount);
    }
}