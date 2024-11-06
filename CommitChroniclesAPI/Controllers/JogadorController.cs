using Microsoft.AspNetCore.Mvc;
using CommitChroniclesAPI.Services;

namespace CommitChroniclesAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JogadorController : ControllerBase
    {
        private readonly JogadorService _jogadorService;

        public JogadorController(JogadorService jogadorService)
        {
            _jogadorService = jogadorService;
        }

        // Endpoint para adicionar um novo jogador
        [HttpPost]
        public async Task<IActionResult> AdicionarJogador([FromBody] Jogador jogador)
        {
            try
            {
                await _jogadorService.AdicionarJogadorAsync(jogador);
                return CreatedAtAction(nameof(ObterJogadorPorId), new { id = jogador.Id }, jogador);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        // Endpoint para buscar todos os jogadores
        [HttpGet]
        public async Task<ActionResult<List<Jogador>>> ObterTodosJogadores()
        {
            var jogadores = await _jogadorService.ObterTodosJogadoresAsync();
            return Ok(jogadores);
        }

        // Endpoint para buscar um jogador por ID
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<Jogador>> ObterJogadorPorId(Guid id)
        {
            try
            {
                var jogador = await _jogadorService.ObterJogadorPorIdAsync(id);
                return Ok(jogador);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // Endpoint para atualizar um jogador por ID
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> AtualizarJogador(Guid id, [FromBody] Jogador jogadorAtualizado)
        {
            try
            {
                await _jogadorService.AtualizarJogadorAsync(id, jogadorAtualizado);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // Endpoint para remover um jogador por ID
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> RemoverJogador(Guid id)
        {
            try
            {
                await _jogadorService.RemoverJogadorAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
