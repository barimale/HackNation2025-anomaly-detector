import { Cell, Pie, PieChart, PieLabelRenderProps, Legend } from 'recharts';

export interface Data{
    name: string;
    value: number;
    color: 'red' | 'green'
}

const RADIAN = Math.PI / 180;

const renderCustomizedLabel = ({ cx, cy, midAngle, innerRadius, outerRadius, percent }: PieLabelRenderProps) => {
  if (cx == null || cy == null || innerRadius == null || outerRadius == null) {
    return null;
  }
  const radius = innerRadius + (outerRadius - innerRadius) * 0.5;
  const ncx = Number(cx);
  const x = ncx + radius * Math.cos(-(midAngle ?? 0) * RADIAN);
  const ncy = Number(cy);
  const y = ncy + radius * Math.sin(-(midAngle ?? 0) * RADIAN);

  return (
    <text x={x} y={y} fill="white" textAnchor={x > ncx ? 'start' : 'end'} dominantBaseline="central">
      {`${((percent ?? 1) * 100).toFixed(0)}%`}
    </text>
  );
};

interface PieChartWithCustomizedLabelProps{
    isAnimationActive: boolean;
    data: any[];
}

function PieChartWithCustomizedLabel(props: PieChartWithCustomizedLabelProps) {
  return (
    <PieChart style={{ height: '400px', width: '100%', maxWidth: '800px', maxHeight: '220vh', aspectRatio: 1 }} responsive>
      <Pie
        data={props.data}
        labelLine={false}
        label={renderCustomizedLabel}
        dataKey="value"
        isAnimationActive={props.isAnimationActive}
      >
          <>
          <Legend />
        {props.data.map((entry, index) => (
          <Cell key={`cell-${entry.name}`} fill={entry.color} />
        ))}
          </>
      </Pie>
    </PieChart>
  );
}

export default PieChartWithCustomizedLabel;
