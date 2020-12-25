﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace lab_oop_8
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            storag.AddObserver(tree);
        }
        TreeViews tree = new TreeViews();
        int p = 0;
        static int k = 5;
        Storage storag = new Storage(k);
        static int index = 0; // Кол-во нарисованных фигур
        static int indexin = 0; // Индекс, в какое место была помещена фигура
        int figure_now = 1; // Какая фигура выбрана
        public interface IObservable
        {   // Наблюдаемый объект
            void AddObserver(IObserver o);
            void RemoveObserver(IObserver o);
            void NotifyObservers();
        }
        public interface IObserver
        {   // Наблюдатель
            void Update(ref TreeView treeView, Storage stg);
        }

        public class Figure
        {
            public int x, y;
            public Color color = Color.Navy;
            public Color fillcolor;

            public Figure() { }
            public virtual string save() { return ""; }
            public virtual void load(string x, string y, string c, string fillcolor) { }
            public virtual void load(ref StreamReader sr, Figure figure, CreateFigure createFigure) { }
            public virtual void GroupAddFigure(ref Figure object1) { }
            public virtual void UnGroup(ref Storage stg, int c) { }
            public virtual void paint_figure(Pen pen, Panel paint_box) { }
            public virtual void move_x(int x, Panel paint_box) { }
            public virtual void move_y(int y, Panel paint_box) { }
            public virtual void changesize(int size) { }
            public virtual bool checkfigure(int x, int y) { return false; }
            public virtual void setcolor(Color color) { }
            public virtual void caseswitch(ref StreamReader sr, ref Figure figure, CreateFigure createFigure) { }
            public virtual void get_min_x(ref int f) { }
            public virtual void get_max_x(ref int f) { }
            public virtual void get_min_y(ref int f) { }
            public virtual void get_max_y(ref int f) { }
            public virtual string name() { return "null"; }
        }

        class Group : Figure
        {
            public int maxcount = 10;
            public Figure[] group;
            public int count;
            int min_x = 99999, max_x = 0, min_y = 99999, max_y = 0;
            public Group()
            {   // Выделяем maxcount мест в хранилище
                count = 0;
                group = new Figure[maxcount];
                for (int i = 0; i < maxcount; ++i)
                    group[i] = null;
            }
            public override string save()
            {
                string str = "Group" + "\n" + count;
                for (int i = 0; i < count; ++i)
                    str += "\n" + group[i].save();
                return str;
            }
            public override void load(ref StreamReader sr, Figure figure, CreateFigure createFigure)
            {
                int chislo = Convert.ToInt32(sr.ReadLine());
                for (int i = 0; i < chislo; ++i)
                {
                    createFigure.caseswitch(ref sr, ref figure, createFigure);
                    GroupAddFigure(ref figure);
                }
            }
            public override void GroupAddFigure(ref Figure object1)
            {
                if (count >= maxcount)
                    return;
                group[count] = object1;
                ++count;
            }
            public override void UnGroup(ref Storage stg, int c)
            {
                stg.delete_object(c);
                for (int i = 0; i < count; ++i)
                {
                    stg.add_object(index, ref group[i], k, ref indexin);
                }
            }
            public override void paint_figure(Pen pen, Panel paint_box)
            {
                for (int i = 0; i < count; ++i)
                {
                    group[i].paint_figure(pen, paint_box);
                }
            }



            public void getsize()
            {
                min_x = 99999; max_x = 0; min_y = 99999; max_y = 0;
                for (int i = 0; i < count; ++i)
                {
                    int f = 0;
                    group[i].get_min_x(ref f);
                    if (f < min_x)
                        min_x = f;
                    group[i].get_max_x(ref f);
                    if (f > max_x)
                        max_x = f;
                    group[i].get_min_y(ref f);
                    if (f < min_y)
                        min_y = f;
                    group[i].get_max_y(ref f);
                    if (f > max_y)
                        max_y = f;
                }
            }



            public override void move_x(int x, Panel paint_box)
            {
                //for (int i = 0; i < count; ++i)
                //{
                //    group[i].move_x(x, paint_box);
                //}
                getsize();
                if ((min_x + x) > 0 && (max_x + x) < paint_box.ClientSize.Width)
                {
                    for (int i = 0; i < count; ++i)
                    {
                        group[i].move_x(x, paint_box);
                    }
                }
            }



            public override void get_min_x(ref int f)
            {
                f = min_x;
            }
            public override void get_max_x(ref int f)
            {
                f = max_x;
            }
            public override void get_min_y(ref int f)
            {
                f = min_y;
            }
            public override void get_max_y(ref int f)
            {
                f = max_y;
            }



            public override void move_y(int y, Panel paint_box)
            {
                //for (int i = 0; i < count; ++i)
                //{
                //    group[i].move_y(y, paint_box);
                //}
                getsize();
                if ((min_y + y) > 0 && (max_y + y) < paint_box.ClientSize.Height)
                {
                    for (int i = 0; i < count; ++i)
                    {
                        group[i].move_y(y, paint_box);
                    }
                }
            }
            public override void changesize(int size)
            {
                for (int i = 0; i < count; ++i)
                {
                    group[i].changesize(size);
                }
            }

            public override bool checkfigure(int x, int y)
            {
                for (int i = 0; i < count; ++i)
                {
                    if (group[i].checkfigure(x, y))
                        return true;
                }
                return false;
            }
            public override void setcolor(Color color)
            {
                for (int i = 0; i < count; ++i)
                {
                    group[i].setcolor(color);
                }
            }


            public override string name()
            {
                return "Group";
            }
        }
        public class CreateFigure : Figure
        {
            public override void caseswitch(ref StreamReader sr, ref Figure figure, CreateFigure createFigure)
            {
                string str = sr.ReadLine();
                switch (str)
                {   // В зависимости какая фигура выбрана
                    case "Circle":
                        figure = new Circle();
                        figure.load(sr.ReadLine(), sr.ReadLine(), sr.ReadLine(), sr.ReadLine());
                        break;
                    case "Line":
                        figure = new Line();
                        figure.load(sr.ReadLine(), sr.ReadLine(), sr.ReadLine(), sr.ReadLine());
                        break;
                    case "Group":
                        figure = new Group();
                        figure.load(ref sr, figure, createFigure);
                        break;
                }
            }
        }
        class Circle : Figure
        {
            public int rad; // Радиус круга
            public Circle() { }
            public Circle(int x, int y, int rad)
            {
                this.rad = rad;
                this.x = x - rad;
                this.y = y - rad;
            }
            public override string save()
            {
                return "Circle" + "\n" + x + "\n" + y + "\n" + rad + "\n" + fillcolor.ToArgb().ToString();
            }
            public override void load(string x, string y, string rad, string fillcolor)
            {
                this.x = Convert.ToInt32(x);
                this.y = Convert.ToInt32(y);
                this.rad = Convert.ToInt32(rad);
                this.fillcolor = Color.FromArgb(Convert.ToInt32(fillcolor));
            }
            public override void paint_figure(Pen pen, Panel paint_box)
            {
                SolidBrush figurefillcolor = new SolidBrush(fillcolor);
                paint_box.CreateGraphics().DrawEllipse(
                    pen, x, y, rad * 2, rad * 2);
                paint_box.CreateGraphics().FillEllipse(
                    figurefillcolor, x, y, rad * 2, rad * 2);
            }

            public override void get_min_x(ref int f)
            {
                f = x;
            }
            public override void get_max_x(ref int f)
            {
                f = x + (rad * 2);
            }
            public override void get_min_y(ref int f)
            {
                f = y;
            }
            public override void get_max_y(ref int f)
            {
                f = y + (rad * 2);
            }


            public override void move_x(int x, Panel paint_box)
            {
                int c = this.x + x;
                int gran = paint_box.ClientSize.Width - (rad * 2);
                check(c, x, gran, gran - 2, ref this.x);
            }
            public override void move_y(int y, Panel paint_box)
            {
                int c = this.y + y;
                int gran = paint_box.ClientSize.Height - (rad * 2);
                check(c, y, gran, gran - 2, ref this.y);
            }
            public override void changesize(int size)
            {
                rad += size;
            }
            public override bool checkfigure(int x, int y)
            {
                return ((x - this.x - rad) * (x - this.x - rad) + (y - this.y - rad) *
                    (y - this.y - rad)) < (rad * rad);
            }
            public override void setcolor(Color color)
            {
                fillcolor = color;
            }

            public override string name()
            {
                return "Circle";
            }
            //~Circle() { }
        }

        class Line : Figure
        {
            //public int x, y;
            public int lenght = 60;
            public int wight = 5;
            public Line() { }
            public Line(int x, int y)
            {
                this.x = x - lenght / 2;
                this.y = y;
            }
            public override string save()
            {
                return "Line" + "\n" + x + "\n" + y + "\n" + lenght + "\n" + fillcolor.ToArgb().ToString();
            }
            public override void load(string x, string y, string lenght, string fillcolor)
            {
                this.x = Convert.ToInt32(x);
                this.y = Convert.ToInt32(y);
                this.lenght = Convert.ToInt32(lenght);
                this.fillcolor = Color.FromArgb(Convert.ToInt32(fillcolor));
            }
            public override void paint_figure(Pen pen, Panel paint_box)
            {
                SolidBrush figurefillcolor = new SolidBrush(fillcolor);
                paint_box.CreateGraphics().DrawRectangle(pen, x,
                                        y, lenght, wight);
                paint_box.CreateGraphics().FillRectangle(figurefillcolor, x,
                    y, lenght, wight);
            }


            public override void get_min_x(ref int f)
            {
                f = x;
            }
            public override void get_max_x(ref int f)
            {
                f = x + lenght;
            }
            public override void get_min_y(ref int f)
            {
                f = y;
            }
            public override void get_max_y(ref int f)
            {
                f = y + wight;
            }

            public override void move_x(int x, Panel paint_box)
            {
                int l = this.x + x;
                int gran = paint_box.ClientSize.Width - lenght;
                check(l, x, gran, --gran, ref this.x);
            }
            public override void move_y(int y, Panel paint_box)
            {
                int l = this.y + y;
                int gran = paint_box.ClientSize.Height - wight;
                check(l, y, gran, --gran, ref this.y);
            }
            public override void changesize(int size)
            {
                lenght += size;
                //wight += size / 5;
            }
            public override bool checkfigure(int x, int y)
            {
                return (this.x <= x && x <= (this.x + lenght) && (this.y - 2) <= y &&
                                    y <= (this.y + wight));
            }
            public override void setcolor(Color color)
            {
                fillcolor = color;
            }

            public override string name()
            {
                return "Line";
            }
        }

        static public void check(int f, int chislo, int gran, int gran1, ref int x)
        {   // Проверка на выход фигуры за границы
            if (f > 0 && f < gran)
                x += chislo;
            else
            {
                if (f <= 0)
                    x = 1;
                else
                    if (f >= gran1)
                    x = gran1;
            }
        }

        public class Storage: IObservable
        {
            public Figure[] objects;
            public TreeView treeView;
            public List<IObserver> observers;
            public Storage() { }
            public Storage(int count)
            {
                objects = new Figure[count];
                observers = new List<IObserver>();
                for (int i = 0; i < count; ++i)
                    objects[i] = null;
            }
            public void intit_tree(ref TreeView treeView)
            {
                this.treeView = treeView;
            }
            public void initialisat(int count)
            {//выделяем место
                objects = new Figure[count];
                for (int i = 0; i < count; ++i)
                    objects[i] = null;
            }
            public void add_object(int ind, ref Figure object1, int count, ref int indexin)
            {//добавляет объект
                while (objects[ind] != null)
                {//если ячейка занята, ищем новое место
                    ind = (ind + 1) % count;
                }
                objects[ind] = object1;
                indexin = ind;
                NotifyObservers();
            }
            public void delete_object(int ind)
            {//удаляет объект из хранилища
                objects[ind] = null;
                if (index > 0)
                    index--;
                NotifyObservers();
            }
            public bool check_empty(int index)
            {//занято ли место
                if (objects[index] == null)
                    return true;
                else
                    return false;
            }
            public int occupied(int size)
            {//кол-во занятых мест
                int count_occupied = 0;
                for (int i = 0; i < size; ++i)
                    if (!check_empty(i))
                        ++count_occupied;
                return count_occupied;
            }
            public void increase(ref int size)
            {//расширение хранилища
                Storage storage1 = new Storage(size + 10);
                for (int i = 0; i < size; ++i)
                    storage1.objects[i] = objects[i];
                for (int i = size; i < (size + 10) - 1; ++i)
                {
                    storage1.objects[i] = null;
                }
                size = size + 10;
                initialisat(size);
                for (int i = 0; i < size; ++i)
                    objects[i] = storage1.objects[i];
            }

            public void AddObserver(IObserver o)
            {
                observers.Add(o);
            }
            public void RemoveObserver(IObserver o)
            {
                observers.Remove(o);
            }
            public void NotifyObservers()
            {
                foreach (IObserver observer in observers)
                    observer.Update(ref treeView, this);
            }

            //~Storage() { }
        };

        class TreeViews : IObserver
        {
            public TreeViews() { }
            public void Update(ref TreeView treeView, Storage stg)
            {
                treeView.Nodes.Clear();
                treeView.Nodes.Add("Figures");
                for (int i = 0; i < k; ++i)
                {
                    if (!stg.check_empty(i))
                    {
                        //treeView.Nodes.Add(stg.objects[i].name());
                        fillnode(treeView.Nodes[0], stg.objects[i]);
                    }
                }
                treeView.ExpandAll();
            }
            public void fillnode(TreeNode treeNode, Figure figure)
            {
                TreeNode nodes = treeNode.Nodes.Add(figure.name());
                if (figure.name() == "Group")
                {
                    for (int i = 0; i < (figure as Group).count; ++i)
                    {
                        fillnode(nodes, (figure as Group).group[i]);
                    }
                }
            }
        }
       
        private void paint_figure(Color name, ref Storage stg, int index)
        {//рисуем круг на панели
            //Pen pen = new Pen(name, 4);
            //SolidBrush figurefillcolor;
            if (!stg.check_empty(index))
            {
                Pen pen = new Pen(name, 4);
                stg.objects[index].color = name;
                stg.objects[index].paint_figure(pen, paint_box);
            }

        }

        private void button_del__item_storage_Click(object sender, EventArgs e)
        {
            remove_selected_circle(ref storag);
            paint_box.Refresh();
            if (storag.occupied(k) != 0)
            {
                for (int i = 0; i < k; ++i)
                {
                    paint_figure(Color.Navy, ref storag, i);
                }
            }
        }

        private void remove_selected_circle(ref Storage stg)
        {//удаляет выделенные элементы
            for (int i = 0; i < k; ++i)
            {
                if (!stg.check_empty(i))
                {
                    if (stg.objects[i].color == Color.Red)
                    {
                        stg.delete_object(i);
                    }
                }
            }
        }

        private void paint_box_MouseClick(object sender, MouseEventArgs e)
        {
            //проверка на наличие круга на данных координатах
            int c = check_figure(ref storag, k, e.X, e.Y);
            storag.intit_tree(ref treeView1);
            if (index == k)
                storag.increase(ref k);
            if (c != -1)
            {//круг уже есть
                if (Control.ModifierKeys == Keys.Control)
                {//если нажат, выделяем несколько объектов
                    if (p == 0)
                    {
                        paint_figure(Color.Navy, ref storag, indexin);
                        p = 1;
                    }
                    paint_figure(Color.Red, ref storag, c);
                }
                else
                {//иначе только один объект
                    remove_selection_circle(ref storag);

                    paint_figure(Color.Red, ref storag, c);
                }
                return;
            }
            else
            {//круга нет
                Figure figure = new Figure();
                switch (figure_now)
                {   // В зависимости какая фигура выбрана
                    case 0:
                        return;
                    case 1:
                        figure = new Circle(e.X, e.Y, 15);
                        break;
                    case 2:
                        figure = new Line(e.X, e.Y);
                        break;
                }
                storag.add_object(index, ref figure, k, ref indexin);

                remove_selection_circle(ref storag);
                storag.objects[indexin].fillcolor = colorDialog1.Color;
                paint_figure(Color.Red, ref storag, indexin);
                ++index;

            }
            p = 0;
        }

        private int check_figure(ref Storage stg, int size, int x, int y)
        {//проверка на наличие круга с координатами в хранилище
            if (stg.occupied(size) != 0)
            {
                for (int i = 0; i < size; ++i)
                {
                    if (!stg.check_empty(i))
                    {
                        if (stg.objects[i].checkfigure(x, y))
                            return i;
                    }
                }
            }
            return -1;
        }

        private void remove_selection_circle(ref Storage stg)
        {//снимает выделение
            for (int i = 0; i < k; ++i)
            {
                if (!stg.check_empty(i))
                {
                    paint_figure(Color.Navy, ref storag, i);
                }
            }
        }


        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {//кнопка Delete
            if (e.KeyCode == Keys.Delete)
            {
                remove_selected_circle(ref storag);
            }
            if (e.KeyCode == Keys.Oemplus)
            {   // Увеличиваем размер фигуры
                changesize(ref storag, 5);
            }
            if (e.KeyCode == Keys.OemMinus)
            {   // Уменьшаем размер фигуры
                changesize(ref storag, -5);
            }
            if (e.KeyCode == Keys.W)
            {   // Перемещение по оси X вверх
                move_y(ref storag, -10);
            }
            if (e.KeyCode == Keys.S)
            {   // Перемещение по оси X вниз
                move_y(ref storag, +10);
            }
            if (e.KeyCode == Keys.A)
            {   // Перемещение по оси Y вниз
                move_x(ref storag, -10);
            }
            if (e.KeyCode == Keys.D)
            {   // Перемещение по оси Y вверх
                move_x(ref storag, +10);
            }
            paint_box.Refresh();
            paint_all(ref storag);
        }

        private void paint_all(ref Storage stg)
        {   // Рисует все фигуры на панели
            for (int i = 0; i < k; ++i)
                if (!stg.check_empty(i))
                    paint_figure(stg.objects[i].color, ref storag, i);
        }

        private void drawellipse_Click(object sender, EventArgs e)
        {
            figure_now = 1;
        }

        private void drawline_Click(object sender, EventArgs e)
        {
            figure_now = 2;
        }

        private void button_color_Click(object sender, EventArgs e)
        {//выбор цвета
            if (colorDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            button_color.BackColor = colorDialog1.Color;
            for (int i = 0; i < k; ++i)
            {
                if (!storag.check_empty(i))
                    if (storag.objects[i].color == Color.Red)
                    {
                        storag.objects[i].setcolor(colorDialog1.Color);
                        paint_figure(storag.objects[i].color, ref storag, i);
                    }
            }


        }

        private void changesize(ref Storage stg, int size)
        {   // Увеличивает или уменьшает размер фигур, в зависимости от size
            for (int i = 0; i < k; ++i)
            {
                if (!stg.check_empty(i))
                {   // Если под i индексом в хранилище есть объект
                    if (stg.objects[i].color == Color.Red)
                    {
                        stg.objects[i].changesize(size);
                    }
                }
            }
        }

        private void move_y(ref Storage stg, int y)
        {   // Функция для перемещения фигур по оси Y
            for (int i = 0; i < k; ++i)
            {
                if (!stg.check_empty(i))
                {
                    if (stg.objects[i].color == Color.Red)
                    {
                        stg.objects[i].move_y(y, paint_box);
                    }
                }
            }
        }

        private void move_x(ref Storage stg, int x)
        {   // Функция для перемещения фигур по оси X
            for (int i = 0; i < k; ++i)
            {
                if (!stg.check_empty(i))
                {
                    if (stg.objects[i].color == Color.Red)
                    {
                        stg.objects[i].move_x(x, paint_box);
                    }
                }
            }
        }

        private void button_group_Click(object sender, EventArgs e)
        {
            Figure group = new Group();
            for (int i = 0; i < k; ++i)
            {
                if (!storag.check_empty(i))
                    if (storag.objects[i].color == Color.Red)
                    {
                        group.GroupAddFigure(ref storag.objects[i]);
                        storag.delete_object(i);
                    }
            }
            storag.add_object(index, ref group, k, ref indexin);
        }

        private void button_ungroup_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < k; ++i)
            {
                if (!storag.check_empty(i))
                    if (storag.objects[i].color == Color.Red)
                    {
                        storag.objects[i].UnGroup(ref storag, i);
                        return;
                    }
            }
        }

        string path = @"C:\Users\ramze\source\repos\lab_oop_8\lab_oop_8\Test.txt";
        private void button_save_Click(object sender, EventArgs e)
        {
            using (StreamWriter sw = new StreamWriter(path, false, System.Text.Encoding.Default))
            {
                sw.WriteLine(storag.occupied(k));
                for (int i = 0; i < k; ++i)
                {
                    if (!storag.check_empty(i))
                    {
                        sw.WriteLine(storag.objects[i].save());
                    }
                }
            }
        }

        private void button_load_Click(object sender, EventArgs e)
        {
            StreamReader sr = new StreamReader(path, System.Text.Encoding.Default);
            {
                string str = sr.ReadLine();
                int strend = Convert.ToInt32(str);
                for (int i = 0; i < strend; ++i)
                {
                    Figure figure = new Figure();
                    CreateFigure create = new CreateFigure();
                    create.caseswitch(ref sr, ref figure, create);
                    if (index == k)
                        storag.increase(ref k);
                    storag.add_object(index, ref figure, k, ref indexin);
                    ++index;
                }
                paint_all(ref storag);
                sr.Close();
            }
        }
    }
}
