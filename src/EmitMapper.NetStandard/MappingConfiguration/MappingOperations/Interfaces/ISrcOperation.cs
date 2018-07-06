namespace EmitMapper.NetStandard.MappingConfiguration.MappingOperations.Interfaces
{
	public interface ISrcOperation : IMappingOperation
	{
		MemberDescriptor Source { get; set; }
	}
}
