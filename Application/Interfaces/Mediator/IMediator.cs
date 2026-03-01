namespace Application.Interfaces.Mediator
{
    /// <summary>
    /// Abstração para enviar comandos e consultar queries no estilo mediator da aplicação.
    /// </summary>
    public interface IMediator
    {
        /// <summary>
        /// Envia um comando que não retorna resultado.
        /// </summary>
        /// <typeparam name="TCommand">Tipo do comando.</typeparam>
        /// <param name="command">Instância do comando a ser enviada.</param>
        /// <param name="cancellationToken">Token que pode cancelar a operação.</param>
        Task Send<TCommand>(TCommand command, CancellationToken cancellationToken) where TCommand : ICommand;

        /// <summary>
        /// Envia um comando que retorna um resultado.
        /// </summary>
        /// <typeparam name="TCommand">Tipo do comando.</typeparam>
        /// <typeparam name="TResult">Tipo do resultado esperado.</typeparam>
        /// <param name="command">Instância do comando a ser enviada.</param>
        /// <param name="cancellationToken">Token que pode cancelar a operação.</param>
        /// <returns>Uma tarefa contendo o resultado do comando.</returns>
        Task<TResult> Send<TCommand, TResult>(TCommand command, CancellationToken cancellationToken) where TCommand : ICommand<TResult>;

        /// <summary>
        /// Executa uma consulta (query) e retorna o resultado.
        /// </summary>
        /// <typeparam name="TQuery">Tipo da query.</typeparam>
        /// <typeparam name="TResult">Tipo do resultado esperado.</typeparam>
        /// <param name="query">Instância da query a ser executada.</param>
        /// <param name="cancellationToken">Token que pode cancelar a operação.</param>
        /// <returns>Uma tarefa contendo o resultado da consulta.</returns>
        Task<TResult> Query<TQuery, TResult>(TQuery query, CancellationToken cancellationToken) where TQuery : IQuery<TResult>; // Em vez de chamar de Send usamos o Query para não dar conflito
    }
}
