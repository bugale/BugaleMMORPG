using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugaleMMORPGServer {
    public enum Direction {
        DirectionRight = 1,
        DirectionLeft = -1
    }

    public class WorldController {
        private WorldController _instance;

        private WorldController() { }
        public WorldController Instance => this._instance ?? (this._instance = new WorldController());
        
        private void UpdateMovement(Player p) {
            lock (p.UpdateLock) {
                if (!p.IsMoving) return;
                var now = DateTime.Now;
                var diff = (now - p.LastMovementUpdate).TotalSeconds;
                var newLocationX = p.LocationX + diff*p.Character.Speed*(double)p.Direction;
                var newLocationY = p.LocationY + p.VelocityY*diff + p.Character.Map.Gravity*diff*diff;
                var newVelocityY = p.VelocityY + p.Character.Map.Gravity*diff;
            }
        }

        private double GetCollisionTime(Player player, double diff, Platform platform) {
            if ((Math.Abs(platform.Location1Y - platform.Location2Y) > double.Epsilon) &&
                (Math.Abs(platform.Location1X - platform.Location2X) > double.Epsilon)) {
                throw new NotImplementedException();
                // TODO: Add support for diagonal platforms
            }

            if (Math.Abs(platform.Location1X - platform.Location2X) > double.Epsilon) {
                // Horizontal platform

                // Quadratic equation variables
                var a = player.Character.Map.Gravity;
                var b = player.VelocityY;
                var c = player.LocationY - platform.Location1Y;

                if (b*b - 4*a*c < 0 || Math.Abs(a) < double.Epsilon) {
                    // No solution, no collision
                    return -1;
                }

                var t1 = (-b - Math.Sqrt(b*b - 4*a*c))/2*a;
                var t2 = (-b + Math.Sqrt(b*b - 4*a*c))/2*a;

                if (t1 > 0) {
                    var x = player.LocationX + t1*player.Character.Speed*(double) player.Direction;
                    if ((x > platform.Location1X && x < platform.Location2X) ||
                        (x > platform.Location2X && x < platform.Location1X)) {
                        // Collision detected
                        return t1;
                    }
                }
                if (t2 > 0) {
                    var x = player.LocationX + t2*player.Character.Speed*(double) player.Direction;
                    if ((x > platform.Location1X && x < platform.Location2X) ||
                        (x > platform.Location2X && x < platform.Location1X)) {
                        // Collision detected
                        return t2;
                    }
                }

                // No soolution in range, no collision
                return -1;
            } else if (Math.Abs(platform.Location1Y - platform.Location2Y) > double.Epsilon) {
                // Vertical platform

                var t = (platform.Location1X - player.LocationX)/(player.Character.Speed*(double) player.Direction);

                if (t < 0) {
                    // No solution in time, no collision
                    return -1;
                }

                var y = player.LocationY + player.VelocityY*t + player.Gravity*t;
            }
    }
    public abstract class Movable {
        public abstract double LocationX { get; set; }
        public abstract double LocationY { get; set; }
        public abstract double VelocityY { get; set; }
        public abstract double VelocityX { get; }
        public abstract double Gravity { get; set; }
        public abstract bool IsMoving { get; set; }
        public abstract Direction Direction { get; set; }
        public abstract DateTime LastMovementUpdate { get; set; }
        public abstract object UpdateLock { get; set; }

        public double FutureLocationX(double time) {
            return this.LocationX + 
        }
    }
    public class Player : Movable {
        public Player(Character character) {
            this.Character = character;
        }

        public Character Character { get; }
        public override double VelocityX => this.Character.Speed; //TODO: Multiply by direction and is moving
    }
}
