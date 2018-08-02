using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentBusinessLogic.Models;

namespace TalentBusinessLogic.ModelBuilders
{
    #region IModelBuilderAction Interface

    /// <summary>
    /// This serves as a base interface to enable functionality wise action 
    /// to be performed through dependency injection. Its a generic interface 
    /// that provides template types for input model & view model. 
    /// </summary>
    /// <typeparam name="T1">Type of the input model</typeparam>
    /// <typeparam name="T2">Type of the view model</typeparam>
    public interface IModelBuilderAction<T1, T2>
        where T1 : BaseInputModel
        where T2 : BaseViewModel
    {
        /// <summary>
        /// Action performed on the model builder
        /// </summary>
        /// <param name="inputModel"></param>
        /// <param name="viewModel"></param>
        void Populate(T1 inputModel, T2 viewModel);
    }

    #endregion

    #region ModelBuilder Class

    /// <summary>
    /// This class works as a bridge between presentation
    /// layer and specific action being executed by the model builder.
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    public class ModelBuilder
    {
        /// <summary>
        /// This method performs the validation, and if it passes through
        /// without errors it invokes the action
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="inputModel"></param>
        /// <param name="modelBuilderAction"></param>
        /// <returns></returns>
        public T2 Call<T1, T2>(T1 inputModel, IModelBuilderAction<T1, T2> modelBuilderAction) 
        where T1 : BaseInputModel
        where T2 : BaseViewModel
        {
            T2 viewModel = (T2)Activator.CreateInstance(typeof(T2), new object[] { });
            ValidateModel(inputModel, viewModel);

            if (viewModel.Error == null
                || (!viewModel.Error.HasError && viewModel.Error.Count == 0))
            {
                modelBuilderAction.Populate(inputModel, viewModel);
            }

            return viewModel;
        }

        /// <summary>
        /// This method performs the validation of the input model & provides the
        /// details of the error in the view model
        /// </summary>
        /// <param name="inputModel"></param>
        /// <param name="viewModel"></param>
        private void ValidateModel(BaseInputModel inputModel, BaseViewModel viewModel)
        {
            var results = new List<ValidationResult>();
            var context = new ValidationContext(inputModel, null, null);
            if (!Validator.TryValidateObject(inputModel, context, results, true))
            {
                viewModel.Error = new ErrorModel();
                viewModel.Error.AddRange(results.Select(vr => vr.ErrorMessage));
            }
        }
    }

    #endregion

}
