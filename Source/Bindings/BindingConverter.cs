/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET
  (c)opyright RemObjects Software, LLC. 2003-2016. All rights reserved.
---------------------------------------------------------------------------*/

namespace RemObjects.InternetPack
{
	#if ECHOES
	class BindingConverter : ExpandableObjectConverter
	{
		public override Boolean CanConvertFrom(ITypeDescriptorContext context, Type type)
		{
			return base.CanConvertFrom(context, type);
		}

		public override Object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, Object value, Type destinationType)
		{
			if (destinationType == typeof(String) && (value is Binding))
			{
				Binding lBinding = (Binding)value;
				return lBinding.Address + ":" + lBinding.Port;
			}

			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
	#endif
}