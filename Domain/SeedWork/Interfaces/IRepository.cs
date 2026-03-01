using System.Linq.Expressions;

namespace Domain.SeedWork.Interfaces
{
    /// <summary>
    /// Define um contrato genérico para um repositório que fornece operações de acesso a dados para entidades do tipo T.
    /// Suporta consulta, adição e deleção de entidades, além de verificação de existência e recuperação por identificador.
    /// </summary>
    /// <remarks>
    /// Esta interface abstrai padrões comuns de acesso a dados para trabalhar com entidades em uma fonte de persistência.
    /// Implementações normalmente suportam operações assíncronas para escalabilidade e podem ser usadas em conjunto com o padrão Unit of Work.
    /// O repositório não impõe como as entidades são armazenadas ou recuperadas, permitindo flexibilidade nas fontes de dados subjacentes.
    /// </remarks>
    /// <typeparam name="T">O tipo de entidade gerenciado pelo repositório. Deve ser um tipo de referência.</typeparam>
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// Recupera assincronamente uma entidade do tipo T pelo seu identificador único.
        /// </summary>
        /// <param name="id">O identificador único da entidade a ser recuperada.</param>
        /// <returns>Uma tarefa que representa a operação assíncrona. O resultado contém a entidade do tipo T se encontrada; caso contrário, null.</returns>
        Task<T> GetByIdAsync(Guid id);

        /// <summary>
        /// Recupera assincronamente todas as entidades do tipo T na fonte de dados.
        /// </summary>
        /// <returns>Uma tarefa que representa a operação assíncrona. O resultado contém uma coleção enumerável de todas as entidades do tipo T.</returns>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// Retorna uma coleção consultável (IQueryable) de todas as entidades do tipo T na fonte de dados.
        /// </summary>
        /// <remarks>
        /// O <see cref="IQueryable{T}"/> retornado permite execução diferida e composição adicional de consultas usando LINQ.
        /// </remarks>
        /// <returns>Um <see cref="IQueryable{T}"/> para consultar as entidades do tipo T.</returns>
        IQueryable<T> GetAllQueryable();

        /// <summary>
        /// Determina assincronamente se alguma entidade corresponde ao predicado especificado.
        /// </summary>
        /// <param name="predicate">Uma expressão que define as condições a serem testadas contra as entidades do tipo T.</param>
        /// <returns>Uma tarefa que representa a operação assíncrona. O resultado é true se pelo menos uma entidade corresponder aos critérios; caso contrário, false.</returns>
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Adiciona a entidade especificada ao conjunto.
        /// </summary>
        /// <param name="entity">A entidade a ser adicionada. Não pode ser nula.</param>
        void Add(T entity);

        /// <summary>
        /// Adiciona a coleção de entidades especificada ao conjunto.
        /// </summary>
        /// <param name="entities">A coleção de entidades a ser adicionada. Não pode ser nula.</param>
        void AddRange(IEnumerable<T> entities);

        /// <summary>
        /// Remove a entidade especificada da fonte de dados.
        /// </summary>
        /// <param name="entity">A entidade a ser removida. Não pode ser nula.</param>
        void Delete(T entity);
    }
}

