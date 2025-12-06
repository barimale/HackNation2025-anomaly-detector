import React from "react";
import "./MessageBox.css"; // Optional styling

// Props definition for type safety
interface MessageBoxProps {
  title?: string;
  message: JSX.Element;
  onConfirm?: () => void;
  onCancel?: () => void;
  confirmText?: string;
  cancelText?: string;
  show: boolean;
}

const MessageBox: React.FC<MessageBoxProps> = ({
  title = "Message",
  message,
  onConfirm,
  onCancel,
  confirmText = "OK",
  cancelText = "Cancel",
  show
}) => {
  if (!show) return null; // Don't render if not visible

  return (
    <div className="messagebox-overlay">
      <div className="messagebox">
        {title && <h2>{title}</h2>}
        <p>{message}</p>
        <div className="messagebox-buttons">
          {onCancel && (
            <button onClick={onCancel} className="cancel-btn">
              {cancelText}
            </button>
          )}
          {onConfirm && (
            <button onClick={onConfirm} className="confirm-btn">
              {confirmText}
            </button>
          )}
        </div>
      </div>
    </div>
  );
};

export default MessageBox;