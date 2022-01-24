# **Đồ Án Paint**

## **Mục lục**
### [Cách chạy chương trình](#run)  
### [Thông tin nhóm](#team)  
### [Chức năng đã thực hiện](#done)  
### [Chức năng chưa thực hiện](#notdone)  
### [Chức năng cộng thêm](#other)
### [Điểm số mong muốn](#score)
### [Video demo](#video)

## **Cách chạy chương trình**<a name="run"></a>
1. **Chuẩn bị file DLL của các Hình vẽ**
    - Build toàn bộ solution để có được file dll của các Hình vẽ
    - Copy các File DLL đặt vào folder DLL của folder chứa file exe.
    - Thầy có thể dùng các DLL nhóm em đã chuẩn bị sẵn trong folder Release
2. **Chạy chương trình Batch Rename**
    - Run project ở chế độ release hay debug đều được
3. **Release**:
    File exe của chương trình đi kèm với 2 folder:
    - DLL: Chức file dll của các Hình vẽ.
    - images: folder chứa các ảnh mà chương trình sử dụng.
## **Thông tin nhóm**<a name="team"></a>
* * *
Nhóm gồm bốn thành viên
- **Nguyễn Hồ Diệu Hương, MSSV: 19120524**
- **Lê Trần Đăng Khoa, MSSV: 19120546**
- **Lê Nguyễn Thảo Mi, MSSV: 19120576**
- **Phạm Sơn Nam, MSSV: 19120596**

Khi có vấn đề về project, thầy có thể liên lạc với nhóm em thông qua Email: <19120546@student.hcmus.edu.vn>
## **Chức năng đã thực hiện**<a name="done"></a>
* * *
### Yêu cầu cốt lõi
1. Các Hình vẽ được load từ file DLL.

2. Người dùng có thể chọn Hình muốn vẽ từ một ListView.

3. Người dùng có thể xem preview của Hình vẽ khi vẽ.

4. Người dùng có thể hoàn thành Hình vẽ của mình và thay đổi của họ trở thành vĩnh viễn với các Hình vẽ trước đó.

5. Các Hình vẽ của người dùng được lưu vào file nhị phân và có thể load lại khi muốn tiếp tục làm việc.

6. Các Hình vẽ có thể được lưu lại dưới cả 3 dạng: PNG, BPM và JPG.
### Các Hình vẽ cơ bản
1. Rectangle - Hình Chữ nhật.

2. Ellipse - Hình Elip.
3. Line - Đường Thẳng.

## **Chức năng chưa thực hiện**<a name="notdone"></a>
* * *
1. Giảm hiện tượng Flickering bằng cách sử dụng Buffer. Chỉ redraw ở khu vực bị ảnh hưởng, không redraw toàn canvas.
## **Chức năng cộng thêm**<a name="other"></a>
* * *
### **Các improvement mà thầy gợi ý đã được thực hiện:** 
1. Cho phép người dùng thay đổi màu, kích thước và style (dash, dot, dash dot dot...) của nét vẽ

2. Chèn Chữ
3. Thêm Hình ảnh vào canvas

5. Hỗ trợ Layer
6. Có thể chọn một phần tử để chỉnh sửa:
    1. Biến đổi theo chiều ngang và dọc.

    2. Xoay.
    3. Drag & Drop.
7. Zooming
8. Cut, Copy, Paste
9. Undo, Redo
10. Tô màu theo ranh giới
### **Các improvement khác:**
1. Sử dụng thư viện HandyControl (https://hosseini.ninja/handycontrol/) để giao diện của ứng dụng trở nên đẹp và thân thiện với người dùng hơn

2. Giao diện ứng dụng được thiết kế responsive, thích hợp với nhiều kích thước cửa sổ khác nhau.
3. Có các chức năng như new, save, save as, open để tạo mới, lưu, tạo bản sao và mở các project. 
4. Ngoài việc ấn các nút tương ứng để thực hiện các chức năng vừa kể, người dùng cỏ thể ấn ***Crtl + N***, ***Ctrl + S***, ***Ctrl + O***, để thực hiện các chức năng new, save, open.
5. Ngoài việc ấn các nút Undo, Redo trên giao diện thì người dùng có thể ấn các tổ hợp phím ***Crtl + N*** (Undo), ***Ctrl + Y***(Redo)
5.  Có thêm một số Hình vẽ:
    1. **Square**: Hình vuông
    
    2. **Circle**: Hình tròn
    3. **Isosceles Triangle**: Tam giác cân
    4. **Right Triangle**: Tam giác vuông
    5. **Diamond**: Hình Diamond
    6. **Pentagon**: Ngủ giác
    7. **Hexagon**: Lục giác
6. Có chức năng Pen để người dùng vẽ tùy ý.

## **Điểm số mong muốn** <a name="score"></a>
* * *
Điểm số mong muốn của từng thành viên:
- Nguyễn Hồ Diệu Hương (MSSV: 19120224): **10**

- Lê Trần Đăng Khoa (MSSV: 19120546): **10**
- Lê Nguyễn Thảo Mi (MSSV: 1912076): **10**
- Phạm Sơn Nam (MSSV: 19120596): **10**
## **Video demo** <a name="video"></a>
* * *
<https://youtube.com>
