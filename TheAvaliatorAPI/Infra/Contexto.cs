using Microsoft.EntityFrameworkCore;
using TheAvaliatorAPI.Model;

namespace TheAvaliatorAPI.Infra
{
    public class Contexto : DbContext
    {

        public Contexto(DbContextOptions<Contexto> options) : base(options)
        {
        }

        public DbSet<AvaliacaoAlunos> AvaliacaoAlunos { get; set; }

        public DbSet<AvaliacaoProfessor> AvaliacaoProfessor { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AvaliacaoAlunos>(entity =>
            {
                entity.ToTable("avalicaoalunos");

                entity.HasKey(e => e.IdAvaliacao);
                entity.Property(e => e.idProfessor).HasColumnName("idprofessor");
                entity.Property(e => e.IdTurma).HasColumnName("Id_Turma");
                entity.Property(e => e.IdTarefa).HasColumnName("Id_Tarefa");
                entity.Property(e => e.Problem).HasColumnName("PROBLEM");
                entity.Property(e => e.Solution).HasColumnName("SOLUTION");
                entity.Property(e => e.IsTeacher).HasColumnName("IS_TEACHER");
                entity.Property(e => e.CyclomaticComplexity).HasColumnName("CYCLOMATIC_COMPLEXITY");
                entity.Property(e => e.ExceededLimitCC).HasColumnName("EXCEEDED_LIMIT_CC");
                entity.Property(e => e.LinesOfCode).HasColumnName("LINES_OF_CODE");
                entity.Property(e => e.ExceededLimitLOC).HasColumnName("EXCEEDED_LIMIT_LOC");
                entity.Property(e => e.LogicalLinesOfCode).HasColumnName("LOGICAL_LINES_OF_CODE");
                entity.Property(e => e.ExceededLimitLLOC).HasColumnName("EXCEEDED_LIMIT_LLOC");
                entity.Property(e => e.SourceLinesOfCode).HasColumnName("SOURCE_LINES_OF_CODE");
                entity.Property(e => e.LimitSLOC).HasColumnName("LIMIT_SLOC");
                entity.Property(e => e.FinalScore).HasColumnName("FINAL_SCORE");

                entity.HasOne(e => e.AvaliacaoProfessor)
                .WithMany(p => p.AvaliacaoAlunos)                  
                .HasForeignKey(e => e.idProfessor)
                .OnDelete(DeleteBehavior.Restrict);


            });

            modelBuilder.Entity<AvaliacaoProfessor>(entity =>
           {
               entity.ToTable("avalicaoprofessor");

               entity.HasKey(e => e.Id);
               entity.Property(e => e.IdProfessor).HasColumnName("idprofessor");
               entity.Property(e => e.Problem).HasColumnName("PROBLEM");
               entity.Property(e => e.Solution).HasColumnName("SOLUTION");
               entity.Property(e => e.IsTeacher).HasColumnName("IS_TEACHER");
               entity.Property(e => e.CyclomaticComplexity).HasColumnName("CYCLOMATIC_COMPLEXITY");
               entity.Property(e => e.ExceededLimitCC).HasColumnName("EXCEEDED_LIMIT_CC");
               entity.Property(e => e.LinesOfCode).HasColumnName("LINES_OF_CODE");
               entity.Property(e => e.ExceededLimitLOC).HasColumnName("EXCEEDED_LIMIT_LOC");
               entity.Property(e => e.LogicalLinesOfCode).HasColumnName("LOGICAL_LINES_OF_CODE");
               entity.Property(e => e.ExceededLimitLLOC).HasColumnName("EXCEEDED_LIMIT_LLOC");
               entity.Property(e => e.SourceLinesOfCode).HasColumnName("SOURCE_LINES_OF_CODE");
               entity.Property(e => e.LimitSLOC).HasColumnName("LIMIT_SLOC");
               entity.Property(e => e.FinalScore).HasColumnName("FINAL_SCORE");
           });
        }

    }
}