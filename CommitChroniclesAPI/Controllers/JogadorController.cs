using CommitChroniclesAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace CommitChroniclesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JogadoresController : ControllerBase
    {
        private readonly JogadorService _jogadorService;

        // Injeção de dependência do JogadorService
        public JogadoresController(JogadorService jogadorService)
        {
            _jogadorService = jogadorService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] JogadorDTO jogadorDTO)
        {
            try
            {
                // Chama o serviço para fazer login e obter o token
                var token = await _jogadorService.LogarJogadorAsync(jogadorDTO);

                // Retorna o token com uma mensagem de sucesso
                return Ok(new { Message = "Login realizado com sucesso", Token = token });
            }
            catch (Exception ex)
            {
                return Unauthorized(new { Message = ex.Message });
            }
        }

        // POST: api/jogadores
        [HttpPost]
        public async Task<IActionResult> AdicionarJogador(JogadorDTO jogadorDTO)
        {
            try
            {
                await _jogadorService.AdicionarJogadorAsync(jogadorDTO);
                return CreatedAtAction(nameof(ObterJogadorPorId), new { id = jogadorDTO.UserEmail }, jogadorDTO);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { mensagem = ex.Message });
            }
        }

        // GET: api/jogadores
        [HttpGet]
        public async Task<ActionResult<List<JogadorDTO>>> ObterJogadores()
        {
            var jogadores = await _jogadorService.ObterTodosJogadoresAsync();
            return Ok(jogadores);
        }

        // GET: api/jogadores/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<JogadorDTO>> ObterJogadorPorId(Guid id)
        {
            try
            {
                var jogador = await _jogadorService.ObterJogadorPorIdAsync(id);
                return Ok(jogador);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { mensagem = ex.Message });
            }
        }

        // PUT: api/jogadores/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> AtualizarJogador(Guid id, JogadorDTO jogadorDTO)
        {
            try
            {
                await _jogadorService.AtualizarJogadorAsync(id, jogadorDTO);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { mensagem = ex.Message });
            }
        }

        // DELETE: api/jogadores/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoverJogador(Guid id)
        {
            try
            {
                await _jogadorService.RemoverJogadorAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { mensagem = ex.Message });
            }
        }
    }
}
