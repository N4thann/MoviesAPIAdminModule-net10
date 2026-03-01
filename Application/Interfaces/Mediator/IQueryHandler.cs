namespace Application.Interfaces.Mediator
{
    /// <summary>
    /// Manipulador responsável por executar queries e retornar resultados.
    /// </summary>
    /// <typeparam name="TQuery">Tipo da query que será tratada.</typeparam>
    /// <typeparam name="TResult">Tipo do resultado retornado pela query.</typeparam>
    public interface IQueryHandler<in TQuery, TResult> where TQuery : IQuery<TResult>
    {
        /// <summary>
        /// Trata a query especificada e retorna o resultado de forma assíncrona.
        /// </summary>
        /// <param name="query">A query a ser processada.</param>
        /// <param name="cancellationToken">Token que pode ser usado para cancelar a operação.</param>
        /// <returns>Uma tarefa que representa a operação assíncrona e contém o resultado.</returns>
        Task<TResult> Handle(TQuery query, CancellationToken cancellationToken);
    }
}
