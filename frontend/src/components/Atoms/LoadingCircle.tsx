// LoadingCircle.tsx
import React from "react";
import "./LoadingCircle.css";

interface LoadingCircleProps {
  size?: number; // diameter in px
  color?: string; // CSS color
}

const LoadingCircle: React.FC<LoadingCircleProps> = ({ size = 40, color = "white" }) => {
  return (
    <div
      className="loading-circle"
      style={{
        width: size,
        height: size,
        borderColor: `${color} transparent transparent transparent`,
      }}
      role="status"
      aria-label="Loading"
    />
  );
};

export default LoadingCircle;
