import http from "../configuration/http-common";

const upload = (id: string, file: File, onUploadProgress: (progressEvent: any) => void): Promise<any> => {
  let formData = new FormData();

  formData.append("uploadedFile", file);

  return http.post("/api/stream", formData, {
    headers: {
      "Content-Type": "multipart/form-data",
      "Accept": "*/*",
      "X-SessionId": id
    },
    onUploadProgress,
  })};

const notify = (id: string): Promise<any> => {
  let formData = new FormData();

  return http.post("/api/notify", formData, {
    headers: {
      "Content-Type": "multipart/form-data",
      "Accept": "*/*",
      "X-SessionId": id
    }
  })};

const getFiles = () : Promise<any> => {
  return http.get("/api/data/get");
};

const FileUploadService = {
  upload,
  notify,
  getFiles,
};

export default FileUploadService;
