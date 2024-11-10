using static DO.Enumes;

namespace DO
{
    public record Volunteer(
    CarType Car, // השתמשנו בשם `Car` כמאפיין מסוג `CarModel`
    string NameOfVolunteer,
    bool IsAvailable = false 
       
     );
        public struct CarType
    {
        public ModelCar ModelCar { get; init; }
        public CarColor CarColor { get; init; }
        public CarType(ModelCar model, CarColor color)
        {
            ModelCar = model;
            CarColor = color;
        }
    }
   
    //public Volunteer(CarType car, string nameOfVolunteer, bool isAvailable = false)
    //{
    //    Car = car;
    //    NameOfVolunteer = nameOfVolunteer;
    //    IsAvailable = isAvailable;
    //}
}
