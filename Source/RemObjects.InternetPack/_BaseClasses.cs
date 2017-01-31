namespace RemObjects.InternetPack
{
	#if ISLAND
	public class Component : Object {}
	public class MarshalByRefObject : Object {}

	public class EventArgs {
		object Sender { get; set; }
	}

	public class SerializableAttribute {}
	public class BrowsableAttribute {}
	#endif
}