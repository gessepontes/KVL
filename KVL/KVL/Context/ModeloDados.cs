namespace KVL.Context
{
    using System.Data.Entity;
    using Models;

    public partial class ModeloDados : DbContext
    {
        public ModeloDados()
            : base("name=ModeloDados")
        {
        }

        public virtual DbSet<Campeonato> Campeonato { get; set; }
        public virtual DbSet<Campo> Campo { get; set; }
        public virtual DbSet<Cartao> Cartao { get; set; }
        public virtual DbSet<Gol> Gol { get; set; }
        public virtual DbSet<GolAmistoso> GolAmistoso { get; set; }
        public virtual DbSet<HorarioDisponivel> HorarioDisponivel { get; set; }
        public virtual DbSet<Inscrito> Inscrito { get; set; }
        public virtual DbSet<Jogador> Jogador { get; set; }
        public virtual DbSet<JogadorInscrito> JogadorInscrito { get; set; }
        public virtual DbSet<JogadorSumula> JogadorSumula { get; set; }
        public virtual DbSet<Login> Login { get; set; }
        public virtual DbSet<PartidaAmistosa> PartidaAmistosa { get; set; }
        public virtual DbSet<PartidaCampeonato> PartidaCampeonato { get; set; }
        public virtual DbSet<Pessoa> Pessoa { get; set; }
        public virtual DbSet<Posicao> Posicao { get; set; }
        public virtual DbSet<PreInscrito> PreInscrito { get; set; }
        public virtual DbSet<Sumula> Sumula { get; set; }
        public virtual DbSet<Time> Time { get; set; }
        public virtual DbSet<CampeonatoGrupo> CampeonatoGrupo { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Campeonato>()
                .Property(e => e.sNome)
                .IsUnicode(false);

            modelBuilder.Entity<Campeonato>()
                .HasMany(e => e.PreInscrito)
                .WithRequired(e => e.Campeonato)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Campo>()
                .Property(e => e.sNome)
                .IsUnicode(false);

            modelBuilder.Entity<Campo>()
                .Property(e => e.sEndereco)
                .IsUnicode(false);

            modelBuilder.Entity<Campo>()
                .Property(e => e.mValor)
                .HasPrecision(19, 4);

            modelBuilder.Entity<Campo>()
                .Property(e => e.mValorMensal)
                .HasPrecision(19, 4);

            modelBuilder.Entity<Campo>()
                .HasMany(e => e.Campeonato)
                .WithRequired(e => e.Campo)
                .HasForeignKey(e => e.iCodCampo)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Campo>()
                .HasMany(e => e.PartidaAmistosa)
                .WithRequired(e => e.Campo)
                .HasForeignKey(e => e.iCodCampo);

            modelBuilder.Entity<Campo>()
                .HasMany(e => e.HorarioDisponivel)
                .WithRequired(e => e.Campo)
                .HasForeignKey(e => e.iCodCampo)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<HorarioDisponivel>()
                .Property(e => e.sHora)
                .IsUnicode(false);

            modelBuilder.Entity<Inscrito>()
                .HasMany(e => e.JogadorInscrito)
                .WithRequired(e => e.Inscrito)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Inscrito>()
                .HasMany(e => e.PartidaCampeonato)
                .WithRequired(e => e.Inscrito)
                .HasForeignKey(e => e.IDInscrito1)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Inscrito>()
                .HasMany(e => e.PartidaCampeonato1)
                .WithRequired(e => e.Inscrito1)
                .HasForeignKey(e => e.IDInscrito2)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Jogador>()
                .HasMany(e => e.JogadorInscrito)
                .WithRequired(e => e.Jogador)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<JogadorInscrito>()
                .HasMany(e => e.JogadorSumula)
                .WithRequired(e => e.JogadorInscrito)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<JogadorSumula>()
                .HasMany(e => e.Cartao)
                .WithRequired(e => e.JogadorSumula)
                .HasForeignKey(e => e.iCodJogadorSumula)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<JogadorSumula>()
                .HasMany(e => e.Gol)
                .WithRequired(e => e.JogadorSumula)
                .HasForeignKey(e => e.iCodJogadorSumula)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Login>()
                .Property(e => e.sLogin)
                .IsUnicode(false);

            modelBuilder.Entity<Login>()
                .Property(e => e.sSenha)
                .IsUnicode(false);

            modelBuilder.Entity<PartidaAmistosa>()
                .Property(e => e.sHoraPartida)
                .IsUnicode(false);

            modelBuilder.Entity<PartidaAmistosa>()
    .HasMany(e => e.GolAmistoso)
    .WithRequired(e => e.PartidaAmistosa)
    .HasForeignKey(e => e.IDPartida)
    .WillCascadeOnDelete(false);


            modelBuilder.Entity<PartidaCampeonato>()
                .Property(e => e.sHoraPartida)
                .IsUnicode(false);

            modelBuilder.Entity<PartidaCampeonato>()
                .HasMany(e => e.Sumula)
                .WithRequired(e => e.PartidaCampeonato)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Pessoa>()
                .Property(e => e.sNome)
                .IsUnicode(false);

            modelBuilder.Entity<Pessoa>()
                .Property(e => e.sTelefone)
                .IsUnicode(false);

            modelBuilder.Entity<Pessoa>()
                .Property(e => e.sCpf)
                .IsUnicode(false);

            modelBuilder.Entity<Pessoa>()
                .Property(e => e.cSexo)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<Pessoa>()
                .Property(e => e.sRg)
                .IsUnicode(false);

            modelBuilder.Entity<Pessoa>()
                .Property(e => e.sFoto)
                .IsUnicode(false);

            modelBuilder.Entity<Pessoa>()
                .Property(e => e.sEmail)
                .IsUnicode(false);

            modelBuilder.Entity<Pessoa>()
                .HasMany(e => e.Jogador)
                .WithRequired(e => e.Pessoa)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Pessoa>()
                .HasMany(e => e.Login)
                .WithRequired(e => e.Pessoa)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Pessoa>()
                .HasMany(e => e.Time)
                .WithRequired(e => e.Pessoa)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Posicao>()
                .Property(e => e.sDescricao)
                .IsUnicode(false);

            modelBuilder.Entity<Posicao>()
                .HasMany(e => e.Jogador)
                .WithRequired(e => e.Posicao)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<PreInscrito>()
                .HasMany(e => e.Inscrito)
                .WithRequired(e => e.PreInscrito)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Sumula>()
                .Property(e => e.sObservacao)
                .IsUnicode(false);

            modelBuilder.Entity<Sumula>()
                .HasMany(e => e.JogadorSumula)
                .WithRequired(e => e.Sumula)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Time>()
                .Property(e => e.sNome)
                .IsUnicode(false);

            modelBuilder.Entity<Time>()
                .Property(e => e.sSimbolo)
                .IsUnicode(false);

            modelBuilder.Entity<Time>()
                .Property(e => e.sObservacao)
                .IsUnicode(false);

            modelBuilder.Entity<Time>()
                .HasMany(e => e.HorarioDisponivel)
                .WithRequired(e => e.Time)
                .HasForeignKey(e => e.iCodTime)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Time>()
                .HasMany(e => e.Jogador)
                .WithRequired(e => e.Time)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Time>()
                .HasMany(e => e.PartidaAmistosa)
                .WithRequired(e => e.Time)
                .HasForeignKey(e => e.IDTime1)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Time>()
                .HasMany(e => e.PartidaAmistosa1)
                .WithRequired(e => e.Time1)
                .HasForeignKey(e => e.IDTime2)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Time>()
                .HasMany(e => e.PreInscrito)
                .WithRequired(e => e.Time)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Inscrito>()
                .HasMany(e => e.CampeonatoGrupo)
                .WithRequired(e => e.Inscrito)
                .WillCascadeOnDelete(false);
        }
    }
}
