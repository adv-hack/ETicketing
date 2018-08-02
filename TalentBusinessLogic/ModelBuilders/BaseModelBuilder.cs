using TalentBusinessLogic.Models;

namespace TalentBusinessLogic.ModelBuilders
{
    public class BaseModelBuilder : BusinessObjects.BusinessObjects
    {
        protected BaseInputModel inputModel;
        public BaseModelBuilder(BaseInputModel inputModel)
        {
            this.inputModel = inputModel;
        }

        public BaseModelBuilder()
        {
        }
    }
}
