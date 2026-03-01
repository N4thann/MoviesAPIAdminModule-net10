namespace Domain.SeedWork.Interfaces
{
    /// <summary>
    /// Representa a entidade raiz de um agregado no contexto de Domain-Driven Design.
    /// </summary>
    /// <remarks>
    /// Uma raiz de agregado é responsável por manter a integridade do agregado e serve como
    /// ponto de entrada para acessar e modificar entidades relacionadas dentro do agregado.
    /// Todas as interações externas com o agregado devem ser realizadas através da raiz do agregado
    /// para garantir consistência.
    /// </remarks>
    public interface IAggregateRoot
    {

    }
}
