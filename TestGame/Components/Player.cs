using AsciiForge.Components.Colliders;
using AsciiForge.Components.Sprites;
using AsciiForge.Engine;

namespace ConsoleGame.Components
{
    public class Player : Component
    {
        private const float _walkSpeed = 10;
        private const float _runSpeed = 20;
        private const float _jumpSpeed = -20;
        private const float _gravity = 30;

        private ICollider _collider;
        private Vector2 _velocity = Vector2.zero;
        private Animator _animator;
        private Sprite _sprite;

        private void Start()
        {
            _collider = entity.FindComponent<ICollider>()!;
            _animator = entity.FindComponent<Animator>()!;
            _sprite = entity.FindComponent<Sprite>()!;
        }
        
        private void Update(float deltaTime)
        {
            int xAxis = (Input.IsKeyDown(Input.Key.RightArrow) ? 1 : 0) - (Input.IsKeyDown(Input.Key.LeftArrow) ? 1 : 0);
            if (xAxis != 0)
            {
                if (Input.IsKeyDown(Input.Key.Ctrl))
                {
                    _velocity.x = xAxis * _runSpeed;
                    _animator.Set("sprPlayerRun");
                }
                else
                {
                    _velocity.x = xAxis * _walkSpeed;
                    _animator.Set("sprPlayerWalk");
                }
            }
            else
            {
                _velocity.x = 0;
                _animator.Set("sprPlayerIdle");
            }
            _sprite.flipHorizontal = _velocity.x < 0;

            if (Input.IsKeyDown(Input.Key.UpArrow) && _collider.PointMeeting("entSolid", new Vector2(transform.position.x, transform.position.y + 1)))
            {
                _velocity.y = _jumpSpeed;
            }
            else
            {
                _velocity.y += _gravity * deltaTime;
            }

            if (_velocity.length != 0)
            {
                if (_collider.PointMeeting("entSolid", new Vector2(transform.position.x + _velocity.x * deltaTime, transform.position.y)))
                {
                    _velocity.x = 0;
                }
                transform.position.x += _velocity.x * deltaTime;

                if (_collider.PointMeeting("entSolid", new Vector2(transform.position.x, transform.position.y + _velocity.y * deltaTime)))
                {
                    _velocity.y = 0;
                }
                transform.position.y += _velocity.y * deltaTime;
            }
            Game.camera.pos = transform.position + new Vector3(0, 0, -10);

            if (Input.IsKeyReleased(Input.Key.Escape))
            {
                Game.Exit();
            }
        }
    }
}
