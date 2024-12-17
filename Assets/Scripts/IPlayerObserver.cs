using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public interface IPlayerObserver
    {
        void OnServe(Vector3 direction, ForceMode mode);
        void OnBallPosition(Player player);
        void OnBallShooter(string player);
        void OnBallCollision();
    }
}
