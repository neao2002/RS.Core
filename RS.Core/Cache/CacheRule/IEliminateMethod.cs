namespace RS.Cache
{
	/// <summary>
	/// 淘汰算法的接口
	/// </summary>
	public interface IEliminateMethod
	{
		/// <summary>
		/// 删除最小权值
		/// </summary>
		void DelPoorWeight();
		/// <summary>
		/// 获得权值最小项的ID
		/// </summary>
		/// <returns></returns>
		object GetPoorWeightID();
		/// <summary>
		/// 添加一条权值纪录
		/// </summary>
		void InsertWeight(object id);
		/// <summary>
		/// 删除某个权值
		/// </summary>
		void DelWeight(object id);
		/// <summary>
		/// 修改权值
		/// </summary>
		void UpdateWeight(object id);
	}
}
