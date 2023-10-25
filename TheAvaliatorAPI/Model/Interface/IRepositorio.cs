using System.Linq.Expressions;

namespace TheAvaliatorAPI.Model.Interface
{
    public interface IRepositorio<T> where T : class
    {
        IQueryable<T> ObterTodos();
        void Adicionar(T entidade);
        void AdicionarConjunto(List<T> entidades);
        void Atualizar(T entidade);
        void AtualizarConjunto(List<T> entidades);
        void Excluir(T entidade);
        IQueryable<T> Obter(Expression<Func<T, bool>> expressao);
        public void DetachEntities(IEnumerable<T> entities);
        int QuantidadeDe(Expression<Func<T, bool>> expressao);

    }
}
