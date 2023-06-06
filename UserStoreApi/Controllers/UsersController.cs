using UserStoreApi.Models;
using UserStoreApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace UserStoreApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly UsersService _UsersService;

    public UsersController(UsersService UsersService) =>
        _UsersService = UsersService;

    [HttpGet]
    public async Task<List<UserModel>> GetWithSearchText([FromQuery] string? searchText="" ) =>
        await _UsersService.GetAsyncWithSearchText(searchText);
    

    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<UserModel>> Get(string id)
    {
        var User = await _UsersService.GetAsync(id);

        if (User is null)
        {
            return NotFound();
        }

        return User;
    }

    [HttpPost]
    public async Task<IActionResult> Post(UserModel newUser)
    {
        await _UsersService.CreateAsync(newUser);

        return CreatedAtAction(nameof(Get), new { id = newUser.Id }, newUser);
    }

    [HttpPut("{id:length(24)}")]
    public async Task<IActionResult> Update(string id, UserModel updatedUser)
    {
        var User = await _UsersService.GetAsync(id);

        if (User is null)
        {
            return NotFound();
        }

        updatedUser.Id = User.Id;

        await _UsersService.UpdateAsync(id, updatedUser);

        return NoContent();
    }

    [HttpDelete("{id:length(24)}")]
    public async Task<IActionResult> Delete(string id)
    {
        var User = await _UsersService.GetAsync(id);

        if (User is null)
        {
            return NotFound();
        }

        await _UsersService.RemoveAsync(id);

        return NoContent();
    }
}