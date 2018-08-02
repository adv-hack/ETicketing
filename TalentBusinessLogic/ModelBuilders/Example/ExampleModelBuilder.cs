using TalentBusinessLogic.Models;

namespace TalentBusinessLogic.ModelBuilders
{
    /// <summary>
    /// This class serves as an example to perform data validation test, and also
    /// depdendency injection test
    /// </summary>
    public class ExampleModelBuilder : BaseModelBuilder
    {
        /// <summary>
        /// Concrete implementation of sample action
        /// </summary>
        public class DITestAction : IModelBuilderAction<DataAnnotationInputModel, DataAnnotationViewModel>
        {
            public void Populate(DataAnnotationInputModel inputModel, DataAnnotationViewModel viewModel)
            {
                viewModel.Status = "DITest:Populate";
            }
        }
    }
}
