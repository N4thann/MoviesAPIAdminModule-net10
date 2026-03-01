namespace Application.Interfaces.Mediator
{
    /// <summary>
    /// Manipulador responsável por executar comandos sem retorno.
    /// </summary>
    /// <typeparam name="TCommand">Tipo do comando a ser manipulado.</typeparam>
    public interface ICommandHandler<in TCommand> where TCommand : ICommand
    {
        /// <summary>
        /// Trata o comando especificado de forma assíncrona.
        /// </summary>
        /// <param name="command">O comando a ser processado.</param>
        /// <param name="cancellationToken">Token que pode ser usado para cancelar a operação.</param>
        Task Handle(TCommand command, CancellationToken cancellationToken);
    }

    /// <summary>
    /// Manipulador responsável por executar comandos que retornam um resultado.
    /// </summary>
    /// <typeparam name="TCommand">Tipo do comando que produz um resultado.</typeparam>
    /// <typeparam name="TResult">Tipo do resultado retornado pelo manipulador.</typeparam>
    public interface ICommandHandler<in TCommand, TResult> where TCommand : ICommand<TResult>
    {
        /// <summary>
        /// Trata o comando especificado e retorna o resultado de forma assíncrona.
        /// </summary>
        /// <param name="command">O comando a ser processado.</param>
        /// <param name="cancellationToken">Token que pode ser usado para cancelar a operação.</param>
        /// <returns>Uma tarefa que representa a operação assíncrona e contém o resultado.</returns>
        Task<TResult> Handle(TCommand command, CancellationToken cancellationToken);
    }
}
