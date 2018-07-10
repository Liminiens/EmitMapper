using EmitMapper.MappingConfiguration.MappingOperations.Interfaces;
using System;

namespace EmitMapper
{
    public class EmitMapperException : ApplicationException
    {
        public EmitMapperException()
        {
        }

        public EmitMapperException(string message)
            : base(message)
        {
        }

        public EmitMapperException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public EmitMapperException(string message, Exception innerException, IMappingOperation mappingOperation)
            : base(
                BuildMessage(message, mappingOperation),
                innerException
                )
        {
        }

        private static string BuildMessage(string message, IMappingOperation mappingOperation)
        {
            return $"{message} {mappingOperation}";
        }
    }
}