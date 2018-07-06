namespace EmitMapper.NetStandard.MappingConfiguration.MappingOperations.Interfaces
{
	public interface IDestOperation : IMappingOperation
	{
		MemberDescriptor Destination { get; set; }
	}
}
