namespace RemObjects.InternetPack.Http;

uses
  RemObjects.Elements.RTL;

type
  ImmutableHttpCookie = public class
  public
	property Values: ImmutableDictionary<String,String> read fValues;
	property Values[aName: not nullable String]: nullable String read fValues[aName]; virtual; default;
	property Count: Integer read fValues.Count;
	property Keys: sequence of String read fValues.Keys;

	property Value: nullable String read if fValues.Count = 1 then fValues[""] write begin
	  fValues.RemoveAll;
	  fValues[""];
	end;

  protected
	fValues := new Dictionary<String,String>;
  end;

  HttpCookie = public class(ImmutableHttpCookie)
  public
	property Domain: nullable String;
	property Path: nullable String;
	property Secure: Boolean;
	property HttpOnly: Boolean;
	property Expires: DateTime;
	property Values[aName: not nullable String]: nullable String read fValues[aName] write fValues[aName]; override; default;

	method GetCookieHeaderString: String;
	begin
	  var lString := new StringBuilder;
	  for each v in Values.Keys index i do begin
		if i > 0 then
		  lString.Append("&");
		if (v = "") and (Values.Count = 1)  then begin
		  lString.Append(Values[v]) // review this
		end
		else begin
		  lString.Append(v);
		  lString.Append("=");
		  lString.Append(Values[v]) // encode?
		end;
	  end;

	  if assigned(Domain) then begin
		lString.Append("; ");
		lString.Append("domain=");
		lString.Append(Domain.ToLowerInvariant);
	  end;

	  lString.Append("; ");
	  lString.Append("path=");
	  lString.Append(coalesce(Path, "/"));

	  if assigned(Expires) then begin
		lString.Append("; ");
		lString.Append("expires=");
		lString.Append(Expires.ToString("ddd, dd-MMM-yyyy HH:mm:ss UTC"));
	  end;

	  if Secure then
		lString.Append("; Secure");

	  if HttpOnly then
		lString.Append("; HttpOnly");

	  result := lString.ToString;
	end;

  end;

  //
  //
  //

  ImmutableHttpCookieCollection = public class
  public

	property Cookies[aName: String]: nullable ImmutableHttpCookie read GetCookie; virtual; default;
	property Count: Integer read fCookies.Count;
	property Keys: sequence of String read fCookies.Keys;

	[&Sequence]
	method GetSequence: sequence of ImmutableHttpCookie; iterator;
	begin
	  for each k in fCookies.Keys.OrderBy(k -> k) do
		yield fCookies[k];
	end;

  protected

	var fCookies := new Dictionary<String, ImmutableHttpCookie>;

	method GetCookie(aName: not nullable String): nullable ImmutableHttpCookie; virtual;
	begin
	  result := fCookies[aName];
	end;

  assembly

	constructor;
	begin
	end;

	constructor(aCookieHeader: nullable String);
	begin
	  //Log($" reading CookieHeader {aCookieHeader}");
	  for each c in aCookieHeader:Split(";") do begin
		var lSplit := c.SplitAtFirstOccurrenceOf("=");
		if lSplit.Count = 2 then begin
		  var lCookie := new HttpCookie;
		  fCookies[lSplit[0].Trim] := lCookie;
		  //Log($"got cookie '{lSplit[0].Trim}'");

		  var lValues := lSplit[1].Trim.Split("&");
		  if (lValues.Count = 1) and not lSplit[1].Contains("=") then begin
			lCookie[""] := lSplit[1].Trim;
			//Log($"got single cookie value '{lSplit[1].Trim}'");
		  end
		  else begin
			for each v in lValues do begin
			  var lSplitValue := v.SplitAtFirstOccurrenceOf("=");
			  if lSplitValue.Count = 2 then begin
				lCookie[lSplitValue[0].Trim] := lSplitValue[1].Trim;
				//Log($"got cookie named value '{lSplitValue[0].Trim}'='{lSplitValue[1].Trim}'");
			  end;
			end;
		  end;
		end;
	  end;
	end;

  end;

  //
  //
  //

  HttpCookieCollection = public class
  public

	property Cookies[aName: String]: nullable HttpCookie read GetCookie write &Add; virtual; default;
	property Count: Integer read fCookies.Count;
	property Keys: sequence of String read fCookies.Keys;

	method &Add(aName: not nullable String; aCookie: nullable HttpCookie);
	begin
	  fCookies[aName] := aCookie;
	end;

	[&Sequence]
	method GetSequence: sequence of HttpCookie; iterator;
	begin
	  for each k in fCookies.Keys.OrderBy(k -> k) do
		yield fCookies[k] as HttpCookie;
	end;

  protected

	method GetCookie(aName: not nullable String): nullable HttpCookie;
	begin
	  result := fCookies[aName];
	  if not assigned(result) then begin
		result := new HttpCookie;
		fCookies[aName] := result;
	  end;
	end;

  private

	var fCookies := new Dictionary<String, HttpCookie>;

  end;
end.