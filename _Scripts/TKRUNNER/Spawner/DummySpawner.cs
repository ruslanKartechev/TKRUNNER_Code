using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using General.Data;
using Dreamteck.Splines;
using System.Threading;
using System.Threading.Tasks;
namespace TKRunner
{
    public class DummySpawner : MonoBehaviour
    {
        private DummyController dummyController;
        private DummySpawnerController boss;
        public void Init(DummySpawnerController _controller, DummyController _dummyController)
        {
            boss = _controller;
            dummyController = _dummyController;
        }

        //public void SpawnOnStart(SpawnCheckpoint start)
        //{
        //    int countPerRow = boss.Data.NumberPerRow;
        //    float colSpacing = boss.Data.ColomnSpacing;
        //    int amount = boss.Data.StartNum;
        //    List<float> percents = GetStartRowPercents((amount / countPerRow)+1,start.spawnPoint);
        //    List<float> offsets = GetRowOffsets(countPerRow, colSpacing);
        //    foreach (float percent in percents)
        //    {
        //        for(int i =0; i<countPerRow; i++)
        //        {
        //            if(amount > 0)
        //            {
        //            //    Debug.Log("OFFset: " + new Vector2(offsets[i], boss.Data.Yoffset));
        //                dummyController.SpawnDummy(percent, new Vector2(offsets[i], boss.Data.Yoffset), GetDummyType());
        //            }
        //            amount--;
        //        }
        //    }

        //}
        private List<float> GetStartRowPercents(int rows, SplineProjector projector)
        {
            float rowSpacing = boss.Data.RowSpacing ;
            List<float> percents = new List<float>(rows);
            for(int i = 0; i < rows; i++)
            {
                percents.Add((float)projector.result.percent + i * rowSpacing);
            }
            return percents;

        }
        private List<float> GetRowOffsets(int countPerRow, float spacing)
        {
            if (countPerRow % 2 != 0)
                countPerRow++;
            List<float> offsets = new List<float>(countPerRow);
            for(int i = 0; i<countPerRow/2; i++)
            {
                offsets.Add( -spacing * (countPerRow/2 -i) );
            }
            for (int i = countPerRow / 2; i < countPerRow; i++)
            {
                offsets.Add(spacing * (i - countPerRow / 2 + 1 ));
            }
            return offsets;

        }

        //public void SpawnOnCP(SplineProjector splineProj, float interval, bool accelerate)
        //{
        //    int amount = Random.Range(boss.Data.number_min, boss.Data.number_max+1);
        //    List<float> positions = GetPosition((float)splineProj.result.percent,
        //        interval / 100, amount);
        //    List<DummyManager> spawned = SpawnDummies(positions);
        //    foreach (DummyManager dummy in spawned)
        //    {
        //        dummy.StartImmidiate();
        //        if (accelerate)
        //        {
        //            dummy.Accelerate(boss.Data.AccelerationTime, boss.Data.StartMaxSpeed, boss.Data.FromBehindDuration);
        //        }
        //    }
        //}

        //private List<DummyManager> SpawnDummies(List<float> positions)
        //{
        //    List<DummyManager> spawned = new List<DummyManager>(positions.Count);

        //    List<Vector2> offsets = GetOffset(positions.Count, boss.Data.trackHalfWidth);

        //    for (int i = 0; i < positions.Count; i++)
        //    {
        //        spawned.Add( dummyController.SpawnDummy(positions[i], offsets[i], GetDummyType()) );
        //    }
        //    return spawned;
        //}

        private string GetDummyType()
        {
            string type = DummyTypes.Normal;
            float rand = Random.Range(0f, 1f);
            if (rand >= boss.Data.GuardedThreshold)
            {
                type = DummyTypes.Guarded;
            }
            return type;
        }
        private List<float> GetPosition(float percent, float length, int amount)
        {
            List<float> positions = new List<float>(amount);

            for(int i = 0; i < amount; i++)
            {
                float pos = Mathf.Lerp(percent - 0.5f * length, percent + 0.5f*length, (float)i / (amount - 1));
                if (pos < 0)
                    pos = 0;
                positions.Add(pos);
            }
            return positions;
        }

        private List<Vector2> GetOffset(int amount, float halfWidth)
        {
            List<Vector2> offsets = new List<Vector2>(amount);
            float y = boss.Data.Yoffset;
            float x = 1;
            for(int i = 0; i<amount; i++)
            {
                if (i < amount / 2)
                {
                    x = Random.Range(-halfWidth, -halfWidth + boss.Data.DeadZone);
                }
                else
                {
                    x = Random.Range(halfWidth - boss.Data.DeadZone, halfWidth);
                }


                Vector2 offset = new Vector2(x,y);
                offsets.Add(offset);
            }

            return offsets;
        }

    }
}