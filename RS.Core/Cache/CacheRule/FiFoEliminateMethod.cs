using System.Collections;
using System;

namespace RS.Core.Cache
{
	/// <summary>
	/// 先进先出的淘汰算法
    /// </summary>
    [Serializable]
    public class FiFoEliminateMethod:IEliminateMethod
	{
		#region IEliminateMethod 成员

		private ArrayList MapOfWeights;

		public FiFoEliminateMethod()
		{
           MapOfWeights=new ArrayList();
		}

		/// <summary>
		/// 删除最先进入缓存的权值
		/// </summary>
		public void DelPoorWeight()
		{
			// TODO:  添加 FiFoEliminateMethod.DelPoorWeight 实现
			if(MapOfWeights.Count==0)return;
			lock(MapOfWeights)
			{
				MapOfWeights.Remove(MapOfWeights[0]);
			}
		}

		/// <summary>
		/// 删除缓存中的某个权值
		/// </summary>
		/// <param name="id"></param>
		public void DelWeight(object id)
		{
			lock(MapOfWeights)
			{
              MapOfWeights.Remove(id);
			}

		}
		/// <summary>
		/// 获得权值最小的项的ID
		/// </summary>
		/// <returns></returns>
		public object GetPoorWeightID()
		{
			// TODO:  添加 FiFoEliminateMethod.GetPoorWeightID 实现
			lock(MapOfWeights)
			{
				if(MapOfWeights.Count==0)return null;
				return  MapOfWeights[0];
			}
		}

		/// <summary>
		/// 添加某项对应的权值
		/// </summary>
		/// <param name="id"></param>
		public void InsertWeight(object id)
		{
			// TODO:  添加 FiFoEliminateMethod.InsertWeight 实现
			lock(MapOfWeights)
			{
				MapOfWeights.Add(id);
			}
		}

		/// <summary>
		/// 修改权值:将该权值放入最后，即最近加入缓存
		/// </summary>
		/// <param name="id"></param>
		public void UpdateWeight(object id)
		{
			// TODO:  添加 FiFoEliminateMethod.UpdateWeight 实现
            lock (MapOfWeights)
            { 
                //先移除该项，再重新加载
                MapOfWeights.Remove(id);
                MapOfWeights.Add(id);
            }
		}

		#endregion
	}
}
