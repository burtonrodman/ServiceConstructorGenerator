using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Extensions.Options
{
  //
  // Summary:
  //     Used to retrieve configured TOptions instances.
  //
  // Type parameters:
  //   TOptions:
  //     The type of options being requested.
  public interface IOptions<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] out TOptions> where TOptions : class
  {
    //
    // Summary:
    //     The default configured TOptions instance
    TOptions Value { get; }
  }
}
