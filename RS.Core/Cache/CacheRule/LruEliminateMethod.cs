using System.Collections;
using System;

namespace RS.Cache
{
	/// <summary>
	/// 最近访问淘汰算法
	/// </summary>
    [Serializable]
    public class LruEliminateMethod:IEliminateMethod
	{
		#region IEliminateMethod 成员
		/// <summary>
		/// 权值对照表
		/// </summary>
        private ArrayList MapOfWeights;
		public LruEliminateMethod()
		{
           MapOfWeights=new ArrayList();
		}
		

		/// <summary>
		/// 删除最小权值
		/// </summary>
		public void DelPoorWeight()
		{
			lock(MapOfWeights)
			{
				if(MapOfWeights.Count==0)return ;
				MapOfWeights.Remove(MapOfWeights[0]);
			}
		}
        
		/// <summary>
		/// 删除某条权值
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
			// TODO:  添加 LruEliminateMethod.GetPoorWeightID 实现
			lock(MapOfWeights)
			{
				if(MapOfWeights.Count==0)return null;
				return  MapOfWeights[0];
			}
		}

		/// <summary>
		/// 添加一条权值纪录
		/// </summary>
		/// <param name="id"></param>
		public void InsertWeight(object id)
		{
			// TODO:  添加 LruEliminateMethod.InsertWeight 实现
			lock(MapOfWeights)
			{
				MapOfWeights.Add(id);
			}
		}

		

		/// <summary>
		/// 修改权值大小
		/// </summary>
		/// <param name="id"></param>
		public void UpdateWeight(object id)
		{
			// TODO:  添加 LruEliminateMethod.UpdateWeight 实现
			lock(MapOfWeights)
			{
				MapOfWeights.Remove(id);
				MapOfWeights.Add(id);
			}
		}

		#endregion

	}


}
