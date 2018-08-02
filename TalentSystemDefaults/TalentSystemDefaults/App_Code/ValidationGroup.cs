using System.Collections.Generic;
using System.Linq;

namespace TalentSystemDefaults
{
    public static class FieldValidation
    {
        public static ValidationGroup Add(List<VG> validationRules, int minLength = default(int), int maxLength = default(int), int minValue = default(int), int maxValue = default(int), string regularExp = null)
		{
            ValidationGroup group = new ValidationGroup();
            group.value = validationRules.Sum(x => System.Convert.ToInt32(x));
			group.MinLength = minLength;
			group.MaxLength = maxLength;
			group.MinValue = minValue;
			group.MaxValue = maxValue;
			group.RegularExp = regularExp;
            return group;
		}
    }

	public enum VG
	{
		Mandatory = 1,
		MinLength = 2,
		MaxLength = 4,
		MinValue = 8,
		MaxValue = 16,
		RegularExp = 32,
		Numeric = 64
	}
	public class ValidationGroup
	{
        
		public int value {get; set; }
		public int MinLength {get; set; }
        public int MaxLength { get; set; }
        public int MinValue { get; set; }
        public int MaxValue { get; set; }
        public string RegularExp { get; set; }

		public bool HasMandatory
		{
			get
			{
				return Has(VG.Mandatory);
			}
		}
		public bool HasMinLength
		{
			get
			{
				return Has(VG.MinLength);
			}
		}
		public bool HasMaxLength
		{
			get
			{
				return Has(VG.MaxLength);
			}
		}
		public bool HasMinValue
		{
			get
			{
				return Has(VG.MinValue);
			}
		}
		public bool HasMaxValue
		{
			get
			{
				return Has(VG.MaxValue);
			}
		}
		public bool HasRegularExp
		{
			get
			{
				return Has(VG.RegularExp);
			}
		}
		public bool HasNumeric
		{
			get
			{
				return Has(VG.Numeric);
			}
		}
		private bool Has(VG vg)
		{
			return (value & System.Convert.ToInt32(vg)) == System.Convert.ToInt32(vg);
		}
	}
}
